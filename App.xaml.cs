protected override void OnStartup(StartupEventArgs e)
{
    base.OnStartup(e);

    // Ensure the required file is copied to the output directory
    string sourcePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "haarcascade_frontalface_alt.xml");
    string destinationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "haarcascade_frontalface_alt.xml");

    if (!System.IO.File.Exists(destinationPath))
    {
        System.IO.File.Copy(sourcePath, destinationPath);
    }
}