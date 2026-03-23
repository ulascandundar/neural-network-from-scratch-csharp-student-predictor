namespace StudentMarksPredictor.Shared.Exceptions;

public class ModelNotFoundException : UnloggableException
{
    public ModelNotFoundException()
        : base("Egitilmis model bulunamadi. Once POST /api/train ile modeli egitin", 404)
    {
    }
}
