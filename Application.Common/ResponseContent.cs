namespace Application.Common;
public class ResponseContent
{
    public ApplicationError? AppError { get; set; }
}

public class ResponseContent<TModel> : ResponseContent
{
    public TModel? Result { get; set; }
}