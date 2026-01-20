using SWD.DAL.Models;

//ALOALO chú ý nè mấy con gà: Đây là interface cho repository quản lý cảm biến và dữ liệu liên quan trong hệ thống IoT.
//Cái này chịu trách nhiệm 80% công việc của đồ án: Nhận dữ liệu, Lưu lịch sử, Lấy dữ liệu vẽ biểu đồ, Lấy luật cảnh báo.

namespace SWD.DAL.Repositories.Interfaces
{
    public interface ISensorRepository
    {
        // 1. Nhóm xử lý luồng dữ liệu (IoT Data Flow)
        Task<Sensor?> GetSensorByIdAsync(int sensorId);
        Task UpdateSensorAsync(Sensor sensor); // Cập nhật số liệu Real-time
        Task AddReadingAsync(Reading reading); // Lưu lịch sử để vẽ Chart

        // 2. Nhóm Cảnh báo (Alert Engine)
        Task<List<AlertRule>> GetActiveRulesBySensorIdAsync(int sensorId); // Lấy luật để check
        Task AddAlertHistoryAsync(AlertHistory history); // Lưu sự cố

        // 3. Nhóm hiển thị App (Dashboard & Chart)
        Task<List<Sensor>> GetAllSensorsWithDetailsAsync(); // Lấy list hiện lên Dashboard
        Task<List<Reading>> GetReadingsForChartAsync(int sensorId, DateTime from, DateTime to); // Lấy data vẽ biểu đồ

        // 4. Nhóm Cấu hình (CRUD Rule) - Để Admin chỉnh sửa luật
        Task CreateRuleAsync(AlertRule rule);
        Task<List<AlertRule>> GetAllRulesAsync();

        // 5. Save Change
        Task SaveChangesAsync();
    }
}