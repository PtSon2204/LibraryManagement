using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers;

/// <summary>
/// Webhook nhận callback từ SePay khi có giao dịch thành công.
/// SePay gọi endpoint này theo cơ chế push khi giao dịch được xác thực.
/// </summary>
[Route("api/sepay")]
[ApiController]
public class SePayWebhookController : ControllerBase
{
    private readonly ILogger<SePayWebhookController> _logger;

    // Cache các transaction đã nhận để client có thể polling kiểm tra
    // Trong production nên dùng Redis hoặc database
    private static readonly Dictionary<string, SePayTransactionRecord> _receivedTransactions
        = new(StringComparer.OrdinalIgnoreCase);
    private static readonly object _lock = new();

    public SePayWebhookController(ILogger<SePayWebhookController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// SePay gọi POST này khi có giao dịch mới.
    /// Tham chiếu: https://docs.sepay.vn/webhook.html
    /// </summary>
    [HttpPost("webhook")]
    public IActionResult ReceiveWebhook([FromBody] SePayWebhookPayload payload)
    {
        try
        {
            _logger.LogInformation("SePay Webhook received: TransactionId={TransactionId}, Content={Content}, Amount={Amount}",
                payload.Id, payload.Content, payload.TransferAmount);

            var record = new SePayTransactionRecord
            {
                TransactionId = payload.Id.ToString(),
                Content = payload.Content ?? string.Empty,
                Amount = payload.TransferAmount,
                AccountNumber = payload.AccountNumber ?? string.Empty,
                ReceivedAt = DateTime.UtcNow
            };

            lock (_lock)
            {
                // Index theo nội dung chuyển khoản để client polling tìm được
                if (!string.IsNullOrWhiteSpace(payload.Content))
                    _receivedTransactions[payload.Content] = record;
                _receivedTransactions[payload.Id.ToString()] = record;
            }

            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing SePay webhook");
            return Ok(new { success = false }); // Trả 200 để SePay không retry
        }
    }

    /// <summary>
    /// Client polling kiểm tra xem giao dịch có nội dung = transferContent đã được nhận chưa.
    /// </summary>
    [HttpGet("check")]
    public IActionResult CheckTransaction([FromQuery] string transferContent, [FromQuery] decimal amount)
    {
        if (string.IsNullOrWhiteSpace(transferContent))
            return BadRequest("transferContent is required.");

        lock (_lock)
        {
            var found = _receivedTransactions.TryGetValue(transferContent, out var record);
            if (found && record != null)
            {
                // Kiểm tra số tiền khớp (cho phép lệch ±1 để tránh lỗi làm tròn)
                var amountMatch = Math.Abs(record.Amount - amount) < 1;
                return Ok(new { received = true, amountMatch, record.ReceivedAt });
            }
        }

        return Ok(new { received = false });
    }

    /// <summary>Xóa transaction đã xử lý khỏi cache</summary>
    [HttpDelete("clear/{transferContent}")]
    public IActionResult ClearTransaction(string transferContent)
    {
        lock (_lock)
        {
            _receivedTransactions.Remove(transferContent);
        }
        return Ok();
    }
}

// ─── Models ──────────────────────────────────────────────────────────────────

/// <summary>Payload từ SePay webhook theo chuẩn tài liệu SePay</summary>
public class SePayWebhookPayload
{
    public long Id { get; set; }
    public string? Gateway { get; set; }
    public string? TransactionDate { get; set; }
    public string? AccountNumber { get; set; }
    public string? SubAccount { get; set; }
    public string? Code { get; set; }
    public string? Content { get; set; }
    public decimal TransferAmount { get; set; }
    public decimal Accumulated { get; set; }
    public string? TransferType { get; set; }
    public string? ReferenceCode { get; set; }
    public string? Description { get; set; }
}

public class SePayTransactionRecord
{
    public string TransactionId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public DateTime ReceivedAt { get; set; }
}
