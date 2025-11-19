# Insert default categories to Supabase via backend API
$API_URL = "https://financialapp-cleanarchitecture.onrender.com/api/Categories"

# Default categories
$categories = @(
    @{name="Ăn uống"; type="Expense"; icon="🍔"; color="#FF6B6B"; isActive=$true},
    @{name="Di chuyển"; type="Expense"; icon="🚗"; color="#4ECDC4"; isActive=$true},
    @{name="Mua sắm"; type="Expense"; icon="🛍️"; color="#FFE66D"; isActive=$true},
    @{name="Giải trí"; type="Expense"; icon="🎮"; color="#95E1D3"; isActive=$true},
    @{name="Y tế"; type="Expense"; icon="🏥"; color="#F38181"; isActive=$true},
    @{name="Giáo dục"; type="Expense"; icon="📚"; color="#AA96DA"; isActive=$true},
    @{name="Nhà cửa"; type="Expense"; icon="🏠"; color="#FCBAD3"; isActive=$true},
    @{name="Tiền điện nước"; type="Expense"; icon="⚡"; color="#A8E6CF"; isActive=$true},
    @{name="Điện thoại/Internet"; type="Expense"; icon="📱"; color="#FFD3B6"; isActive=$true},
    @{name="Quần áo"; type="Expense"; icon="👔"; color="#FFAAA5"; isActive=$true},
    @{name="Sức khỏe"; type="Expense"; icon="💊"; color="#FF8B94"; isActive=$true},
    @{name="Làm đẹp"; type="Expense"; icon="💄"; color="#FFC6C7"; isActive=$true},
    @{name="Du lịch"; type="Expense"; icon="✈️"; color="#FFD93D"; isActive=$true},
    @{name="Quà tặng"; type="Expense"; icon="🎁"; color="#C7CEEA"; isActive=$true},
    @{name="Từ thiện"; type="Expense"; icon="❤️"; color="#FFEAA7"; isActive=$true},
    @{name="Khác"; type="Expense"; icon="📦"; color="#DFE6E9"; isActive=$true},
    
    @{name="Lương"; type="Income"; icon="💰"; color="#00B894"; isActive=$true},
    @{name="Thưởng"; type="Income"; icon="🎉"; color="#FDCB6E"; isActive=$true},
    @{name="Đầu tư"; type="Income"; icon="📈"; color="#6C5CE7"; isActive=$true},
    @{name="Kinh doanh"; type="Income"; icon="💼"; color="#0984E3"; isActive=$true},
    @{name="Quà tặng"; type="Income"; icon="🎁"; color="#FD79A8"; isActive=$true},
    @{name="Thu nhập khác"; type="Income"; icon="💵"; color="#74B9FF"; isActive=$true}
)

Write-Host "`n=== INSERTING DEFAULT CATEGORIES TO SUPABASE ===" -ForegroundColor Cyan
Write-Host "Total categories to insert: $($categories.Count)" -ForegroundColor Yellow

$success = 0
$failed = 0

foreach ($cat in $categories) {
    try {
        $body = $cat | ConvertTo-Json
        $response = Invoke-RestMethod -Uri $API_URL -Method POST -ContentType "application/json" -Body $body
        Write-Host "✓ Created: $($cat.name) ($($cat.type))" -ForegroundColor Green
        $success++
    }
    catch {
        Write-Host "✗ Failed: $($cat.name) - $($_.Exception.Message)" -ForegroundColor Red
        $failed++
    }
    Start-Sleep -Milliseconds 200
}

Write-Host "`n=== SUMMARY ===" -ForegroundColor Cyan
Write-Host "Success: $success" -ForegroundColor Green
Write-Host "Failed: $failed" -ForegroundColor Red

Write-Host "`nVerifying..." -ForegroundColor Yellow
try {
    $allCategories = Invoke-RestMethod -Uri $API_URL -Method GET
    Write-Host "✓ Total categories in database: $($allCategories.Count)" -ForegroundColor Green
}
catch {
    Write-Host "✗ Could not verify: $($_.Exception.Message)" -ForegroundColor Red
}
