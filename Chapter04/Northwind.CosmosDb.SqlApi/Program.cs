// See https://aka.ms/new-console-template for more information
//await CreateCosmosResources();
//await CreateProductItems();

await CreateInsertProductStoredProcedure();
await ExecuteInsertProductStoredProcedure();

await ListProductItems("SELECT p.id, p.productName, p.unitPrice FROM Items p WHERE p.productId = '78'");