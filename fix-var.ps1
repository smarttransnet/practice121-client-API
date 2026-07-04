$dir = "c:\2026\FleeRo\Practice121\API\src\Application"

$file = "$dir\Reports\Queries\GetMonthlyExpenseReports\GetMonthlyExpenseReportsQueryHandler.cs"
(Get-Content $file) -replace 'var query = ', 'IQueryable<Domain.Reports.MonthlyExpenseReport> query = ' -replace 'var reports = ', 'List<MonthlyExpenseReportResponse> reports = ' | Set-Content $file

$file = "$dir\Payroll\Queries\GetSalaryRecords\GetSalaryRecordsQueryHandler.cs"
(Get-Content $file) -replace 'var query = ', 'IQueryable<Domain.Payroll.DriverSalaryRecord> query = ' -replace 'var records = ', 'List<SalaryRecordResponse> records = ' | Set-Content $file

$file = "$dir\Payroll\Commands\AddCommissionItem\AddCommissionItemCommandHandler.cs"
(Get-Content $file) -replace 'var record = ', 'Domain.Payroll.DriverSalaryRecord? record = ' -replace 'var item = ', 'Domain.Payroll.DriverCommissionItem item = ' | Set-Content $file

$file = "$dir\Payroll\Commands\ApproveSalaryRecord\ApproveSalaryRecordCommandHandler.cs"
(Get-Content $file) -replace 'var record = ', 'Domain.Payroll.DriverSalaryRecord? record = ' | Set-Content $file

$file = "$dir\Payroll\Commands\SubmitSalaryRecord\SubmitSalaryRecordCommandHandler.cs"
(Get-Content $file) -replace 'var record = ', 'Domain.Payroll.DriverSalaryRecord? record = ' | Set-Content $file

$file = "$dir\Reports\Commands\GenerateMonthlyExpenseReport\GenerateMonthlyExpenseReportCommandHandler.cs"
(Get-Content $file) -replace 'var existing = ', 'Domain.Reports.MonthlyExpenseReport? existing = ' -replace 'var report = ', 'Domain.Reports.MonthlyExpenseReport report = ' | Set-Content $file

$file = "$dir\Fuel\Allocations\Queries\GetFuelAllocations\GetFuelAllocationsQueryHandler.cs"
(Get-Content $file) -replace 'var query = ', 'IQueryable<Domain.Fuel.FuelCostAllocation> query = ' -replace 'var allocations = ', 'List<FuelAllocationResponse> allocations = ' | Set-Content $file

$file = "$dir\Fuel\Summaries\Queries\GetVehicleFuelSummaries\GetVehicleFuelSummariesQueryHandler.cs"
(Get-Content $file) -replace 'var query = ', 'IQueryable<Domain.Fuel.VehicleFuelSummary> query = ' -replace 'var summaries = ', 'List<VehicleFuelSummaryResponse> summaries = ' | Set-Content $file

$file = "$dir\Fuel\Woqood\Queries\GetWoqoodImportBatch\GetWoqoodImportBatchQueryHandler.cs"
(Get-Content $file) -replace 'var batch = ', 'Domain.Fuel.WoqoodImportBatch? batch = ' | Set-Content $file

$file = "$dir\Fuel\Woqood\Queries\GetWoqoodBatchTransactions\GetWoqoodBatchTransactionsQueryHandler.cs"
(Get-Content $file) -replace 'var transactions = ', 'List<WoqoodFuelTransactionResponse> transactions = ' | Set-Content $file

$file = "$dir\Fuel\Woqood\Queries\GetWoqoodCardMappings\GetWoqoodCardMappingsQueryHandler.cs"
(Get-Content $file) -replace 'var mappings = ', 'List<WoqoodCardMappingResponse> mappings = ' | Set-Content $file

$file = "$dir\Fuel\Woqood\Commands\AllocateWoqoodTransaction\AllocateWoqoodTransactionCommandHandler.cs"
(Get-Content $file) -replace 'var transaction = ', 'Domain.Fuel.WoqoodFuelTransaction? transaction = ' -replace 'var allocation = ', 'Domain.Fuel.FuelCostAllocation allocation = ' | Set-Content $file

$file = "$dir\Fuel\Woqood\Commands\DeactivateWoqoodCardMapping\DeactivateWoqoodCardMappingCommandHandler.cs"
(Get-Content $file) -replace 'var mapping = ', 'Domain.Fuel.WoqoodCardMapping? mapping = ' | Set-Content $file

$file = "$dir\Fuel\Woqood\Commands\UpsertWoqoodCardMapping\UpsertWoqoodCardMappingCommandHandler.cs"
(Get-Content $file) -replace 'var existing = ', 'Domain.Fuel.WoqoodCardMapping? existing = ' -replace 'var newMapping = ', 'Domain.Fuel.WoqoodCardMapping newMapping = ' | Set-Content $file

$file = "$dir\Fuel\Summaries\Commands\RecalculateFuelSummary\RecalculateFuelSummaryCommandHandler.cs"
(Get-Content $file) -replace '\(IApplicationDbContext dbContext\)', '()' | Set-Content $file

$file = "$dir\Fuel\Woqood\Commands\ImportWoqoodBatch\ImportWoqoodBatchCommandHandler.cs"
(Get-Content $file) -replace 'var batch = ', 'Domain.Fuel.WoqoodImportBatch batch = ' | Set-Content $file

$file = "$dir\Fuel\Allocations\Commands\CreateFuelAllocation\CreateFuelAllocationCommandHandler.cs"
(Get-Content $file) -replace 'var allocation = ', 'Domain.Fuel.FuelCostAllocation allocation = ' | Set-Content $file

$file = "$dir\Payroll\Commands\CreateSalaryRecord\CreateSalaryRecordCommandHandler.cs"
(Get-Content $file) -replace 'var record = ', 'Domain.Payroll.DriverSalaryRecord record = ' | Set-Content $file

