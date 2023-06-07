namespace MedicationInventoryManagement.Contracts;

public class BaseResponse
{
    public BaseResponse()
    {
        Errors = new List<ErrorResponse>();
    }

    public List<ErrorResponse> Errors { get; }

    public bool Success => !Errors.Any();

    public void AddError(string error)
    {
        Errors.Add(new ErrorResponse(error));
    }
}