using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Linq; // QUAN TRỌNG: Thư viện để gom nhóm dữ liệu

namespace IoT_Simulator
{
    // 1. Class lưu trữ dữ liệu thời tiết (Dùng để sắp xếp)
    class WeatherRecord
    {
        public DateTime Timestamp { get; set; } // Thời gian chuẩn để sort
        public string TimeString { get; set; }  // Thời gian hiển thị (yyyy-MM-dd HH:mm)
        public string City { get; set; }
        public double Temp { get; set; }
        public double Hum { get; set; }
        public double Light { get; set; }
    }

    // 2. Class cấu hình thiết bị
    class VirtualDevice
    {
        public string Name { get; set; }
        public string MacAddress { get; set; }
        public string Token { get; set; }
        public string TargetCity { get; set; }
    }

    class Program
    {
        // ================= CẤU HÌNH HỆ THỐNG =================
        private const string EOH_API_URL = "https://api.e-ra.io/v1/telemetry";

        // Tốc độ Demo: 3 giây chuyển sang mốc thời gian tiếp theo
        // (Không chờ 30 phút thật để tiết kiệm thời gian thuyết trình)
        private const int SIMULATION_SPEED_MS = 3000;

        // DANH SÁCH THIẾT BỊ
        static List<VirtualDevice> devices = new List<VirtualDevice>
        {
            new VirtualDevice {
                Name = "Tram_HaNoi",
                TargetCity = "Ha Noi",
                MacAddress = "AA:BB:CC:11:22",
                Token = "330b8853-d347-4dd3-86e5-08894e9a5479"
            },
            new VirtualDevice {
                Name = "Tram_HCM",
                TargetCity = "Ho Chi Minh",
                MacAddress = "AA:BB:CC:55:66",
                Token = "a6f9baf5-040d-44de-8eaa-dba53b37f872"
            }
        };

        static async Task Main(string[] args)
        {
            SetupConsole();

            string csvPath = "weather-vn-1.csv";
            if (!File.Exists(csvPath))
            {
                Console.WriteLine($"[LỖI] Không tìm thấy file '{csvPath}'. Hãy Copy file vào thư mục debug!");
                return;
            }

            // BƯỚC 1: ĐỌC VÀ XỬ LÝ FILE (Load vào RAM)
            Console.WriteLine("-> Đang đọc file và đồng bộ hóa dữ liệu (Vui lòng chờ)...");
            var allRecords = LoadAndParseCsv(csvPath);

            // BƯỚC 2: GOM NHÓM THEO THỜI GIAN (Time Slices)
            // Lệnh này giúp Hà Nội và HCM tại cùng 1 thời điểm được gom vào 1 cục
            var timeGroups = allRecords
                .OrderBy(x => x.Timestamp)       // Sắp xếp tăng dần
                .GroupBy(x => x.TimeString)      // Gom những dòng cùng phút vào 1 nhóm
                .ToList();

            Console.WriteLine($"-> Đã xử lý xong: {timeGroups.Count} khung giờ đo.");
            Console.WriteLine("-> Bắt đầu phát dữ liệu đồng bộ...\n");

            using var client = new HttpClient();

            // BƯỚC 3: CHẠY VÒNG LẶP GỬI TIN
            foreach (var group in timeGroups)
            {
                // In ra tiêu đề khung giờ
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"------- ⏰ MỐC THỜI GIAN: {group.Key} -------");

                // Trong khung giờ này có bao nhiêu thành phố thì gửi bấy nhiêu
                foreach (var data in group)
                {
                    // Tìm thiết bị phụ trách thành phố này
                    var dev = devices.FirstOrDefault(d => d.TargetCity == data.City);

                    if (dev != null)
                    {
                        // In ra màn hình (Hà Nội Xanh, HCM Vàng)
                        if (data.City == "Ha Noi") Console.ForegroundColor = ConsoleColor.Cyan;
                        else Console.ForegroundColor = ConsoleColor.Yellow;

                        Console.WriteLine($"   📍 {dev.Name}: {data.Temp}°C | Hum: {data.Hum}% | Light: {data.Light:F0}%");

                        // Gửi dữ liệu lên Server
                        await SendTelemetry(client, dev, data.Temp, data.Hum, data.Light);
                    }
                }

                // Gửi xong cả 2 thành phố rồi mới nghỉ để qua giờ tiếp theo
                Console.WriteLine();
                Thread.Sleep(SIMULATION_SPEED_MS);
            }

            Console.ResetColor();
            Console.WriteLine("ĐÃ CHẠY HẾT DỮ LIỆU!");
            Console.ReadLine();
        }

        // --- HÀM PHỤ TRỢ: ĐỌC CSV ---
        static List<WeatherRecord> LoadAndParseCsv(string path)
        {
            var list = new List<WeatherRecord>();
            using (var reader = new StreamReader(path))
            {
                reader.ReadLine(); // Bỏ header
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    // Regex tách cột CSV chuẩn
                    string[] parts = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

                    try
                    {
                        string city = parts[2].Trim().Replace("\"", "");

                        // Chỉ lấy đúng HN và HCM
                        if (city == "Ha Noi" || city == "Ho Chi Minh")
                        {
                            string rawTime = parts[0].Replace("\"", "");

                            if (DateTime.TryParse(rawTime, out DateTime t))
                            {
                                // Tính toán fake Light
                                double hum = double.Parse(parts[6], CultureInfo.InvariantCulture);
                                double light = (100 - hum) + new Random().Next(-10, 10);
                                if (light < 0) light = 0; if (light > 100) light = 100;

                                list.Add(new WeatherRecord
                                {
                                    Timestamp = t,
                                    // Format lấy đến phút để gom nhóm
                                    TimeString = t.ToString("yyyy-MM-dd HH:mm"),
                                    City = city,
                                    Temp = double.Parse(parts[3], CultureInfo.InvariantCulture),
                                    Hum = hum,
                                    Light = light
                                });
                            }
                        }
                    }
                    catch { /* Bỏ qua dòng lỗi */ }
                }
            }
            return list;
        }

        // --- HÀM PHỤ TRỢ: GỬI API ---
        static async Task SendTelemetry(HttpClient client, VirtualDevice dev, double temp, double hum, double light)
        {
            var payload = new
            {
                mac_address = dev.MacAddress,
                values = new { temperature = temp, humidity = hum, light = Math.Round(light, 0) }
            };

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("X-EOH-Token", dev.Token);

            try
            {
                // Dùng _ = await task để chạy nhưng không cần chờ quá lâu
                var response = await client.PostAsJsonAsync(EOH_API_URL, payload);
                if (!response.IsSuccessStatusCode)
                    Console.Write($" [Lỗi gửi: {response.StatusCode}]");
            }
            catch (Exception ex)
            {
                Console.Write($" [Lỗi mạng: {ex.Message}]");
            }
        }

        static void SetupConsole()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "IoT Simulator - Realtime Sync";
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("==================================================");
            Console.WriteLine("   BỘ GIẢ LẬP ĐỒNG BỘ: HÀ NỘI & HCM (REAL-TIME)   ");
            Console.WriteLine("==================================================");
            Console.ResetColor();
        }
    }
}