using Projects;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Ecom_Api>("gateway");

builder.AddProject<Ecom_OrderProccessor>("orders");

builder.AddProject<Ecom_StockApi>("stock");

builder.Build().Run();
