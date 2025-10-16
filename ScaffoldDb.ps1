dotnet ef dbcontext scaffold "Server=localhost;Database=CP;Trusted_Connection=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer `
    --project Persist.CapacityPlanner `
    --startup-project CapacityPlanner `
    --output-dir DbModel\Entities `
    --context-dir DbModel `
    --context CapacityPlannerContext `
    --no-pluralize `
    --force `
    --no-build `
    --no-onconfiguring `
