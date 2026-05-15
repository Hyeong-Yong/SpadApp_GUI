namespace SpadApp
{
    public partial class MainForm
    {
        private void Log(string msg)
        {
            richtxtLog.AppendText(
                msg + Environment.NewLine);

            // 최대 줄 제한
            if (richtxtLog.Lines.Length > 1000)
            {
                richtxtLog.Lines =
                    richtxtLog.Lines.Skip(100).ToArray();
            }

            richtxtLog.SelectionStart =
                richtxtLog.TextLength;

            richtxtLog.ScrollToCaret();
        }

    }
}