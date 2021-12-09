WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGraphQLServer()
    .AddType<UploadType>()
    .AddQueryType()
        .AddTypeExtension<FileQueries>()
    .AddMutationType()
        .AddTypeExtension<FileMutations>();

WebApplication? app = builder.Build();

app.UseGraphQLAltair();
app.MapGraphQL();
app.Run();


public class FileMutations : ObjectTypeExtension
{
    protected override void Configure(IObjectTypeDescriptor descriptor)
    {
        descriptor.Name(OperationTypeNames.Mutation);

        descriptor
            .Field("uploadFile")
            .Argument("input", a => a.Type<UploadFileInputType>())
            .Type<BooleanType>()
            .Resolve(context =>
            {
                UploadFileInput input = context.ArgumentValue<UploadFileInput>("input");

                return true;
            });
    }
}

public class UploadFileInputType : InputObjectType<UploadFileInput>
{
    protected override void Configure(IInputObjectTypeDescriptor<UploadFileInput> descriptor)
    {
        descriptor.Name("UploadFileInput");

        descriptor.Field(f => f.Key).Type<NonNullType<StringType>>();

        descriptor.Field(f => f.Content).Type<UploadType>();
    }
}

public class UploadFileInput
{
    public string BucketName { get; set; }
    public string Key { get; set; }
    public IFile Content { get; set; }
    public bool? IsPublic { get; set; }
}

public class FileQueries : ObjectTypeExtension
{
    protected override void Configure(IObjectTypeDescriptor descriptor)
    {
        descriptor.Name(OperationTypeNames.Query);

        descriptor
            .Field("getFile")
            .Argument("key", a => a.Type<NonNullType<StringType>>())
            .Type<FileType>()
            .Resolve(async context =>
            {
                throw new NotImplementedException();
            });
    }
}

public class FileType : ObjectType<File>
{
    protected override void Configure(IObjectTypeDescriptor<File> descriptor)
    {

    }
}

public class File
{
    public string BucketName { get; set; }
    public string Key { get; set; }
    public string ContentAsBase64 { get; set; }
    public bool IsPublic { get; set; }
}
