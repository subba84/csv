using System.Windows.Forms;

public class Form1 : Form
{
    private Button helloButton;

    public Form1()
    {
        Text = "Hello World App";
        Width = 300;
        Height = 200;
        StartPosition = FormStartPosition.CenterScreen;

        helloButton = new Button
        {
            Text = "Click Me",
            Width = 120,
            Height = 40,
            Left = (Width - 120) / 2,
            Top = (Height - 80) / 2
        };

        helloButton.Click += (sender, e) =>
        {
            MessageBox.Show("Hello World", "Greeting", MessageBoxButtons.OK, MessageBoxIcon.Information);
        };

        Controls.Add(helloButton);
    }
}
