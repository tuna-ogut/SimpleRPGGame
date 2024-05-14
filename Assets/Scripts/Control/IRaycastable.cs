
namespace RPG.Control
{
    public interface IRaycastable
    {
        bool HandleRaycast(PlayerController caller);
        Cursors GetCursorType();
    }
}

