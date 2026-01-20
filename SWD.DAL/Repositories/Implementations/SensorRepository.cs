using Microsoft.EntityFrameworkCore;
using SWD.DAL.Models;
using SWD.DAL.Repositories.Interfaces;

//ALOALO chú ý nè mấy con gà: code này dùng EF Core để thao tác với DB, bao gồm các phương thức lấy, thêm, cập nhật dữ liệu liên quan đến Sensor, Reading, AlertRule, AlertHistory.
//Cái này chịu trách nhiệm 80% công việc của đồ án: Nhận dữ liệu, Lưu lịch sử, Lấy dữ liệu vẽ biểu đồ, Lấy luật cảnh báo.
namespace SWD.DAL.Repositories.Implementations
{
    public class SensorRepository : ISensorRepository
    {
        private readonly IoTFinalDbContext _context;

        public SensorRepository(IoTFinalDbContext context)
        {
            _context = context;
        }

        // --- 1. XỬ LÝ DỮ LIỆU ---
        public async Task<Sensor?> GetSensorByIdAsync(int sensorId)
        {
            // Lấy Sensor kèm luôn Hub để check trạng thái Online/Offline
            return await _context.Sensors.Include(s => s.Hub).FirstOrDefaultAsync(s => s.SensorId == sensorId);
        }

        public async Task UpdateSensorAsync(Sensor sensor)
        {
            _context.Sensors.Update(sensor);
            // Chưa save ngay để tối ưu transaction bên Service
            await Task.CompletedTask;
        }

        public async Task AddReadingAsync(Reading reading)
        {
            await _context.Readings.AddAsync(reading);
        }

        // --- 2. CẢNH BÁO ---
        public async Task<List<AlertRule>> GetActiveRulesBySensorIdAsync(int sensorId)
        {
            // Chỉ lấy những luật đang Active (IsActive = true)
            return await _context.AlertRules
                .Where(r => r.SensorId == sensorId && r.IsActive == true)
                .ToListAsync();
        }

        public async Task AddAlertHistoryAsync(AlertHistory history)
        {
            await _context.AlertHistories.AddAsync(history);
        }

        // --- 3. DASHBOARD & CHART ---
        public async Task<List<Sensor>> GetAllSensorsWithDetailsAsync()
        {
            // Include hết thông tin liên quan để hiển thị đầy đủ
            return await _context.Sensors
                .Include(s => s.Hub)
                .Include(s => s.Type)
                .OrderBy(s => s.HubId) // Sắp xếp theo Hub cho đẹp
                .ToListAsync();
        }

        public async Task<List<Reading>> GetReadingsForChartAsync(int sensorId, DateTime from, DateTime to)
        {
            // Lấy dữ liệu trong khoảng thời gian user chọn
            return await _context.Readings
                .Where(r => r.SensorId == sensorId && r.RecordedAt >= from && r.RecordedAt <= to)
                .OrderBy(r => r.RecordedAt) // Sắp xếp theo thời gian để vẽ đường cho đúng
                .ToListAsync();
        }

        // --- 4. CONFIG RULE ---
        public async Task CreateRuleAsync(AlertRule rule)
        {
            await _context.AlertRules.AddAsync(rule);
        }

        public async Task<List<AlertRule>> GetAllRulesAsync()
        {
            return await _context.AlertRules.Include(r => r.Sensor).ToListAsync();
        }

        // --- 5. CHỐT ĐƠN (SAVE) ---
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}