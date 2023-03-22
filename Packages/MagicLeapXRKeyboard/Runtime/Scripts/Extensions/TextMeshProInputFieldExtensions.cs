namespace MagicLeap.XRKeyboard.Extensions
{
    using TMPro;

    public static class TextMeshProInputFieldExtensions
    {
        public static bool HasSelection(this TMP_InputField tmpInputField)
        {
           return tmpInputField.selectionStringAnchorPosition != tmpInputField.stringPosition; }
        }
}
