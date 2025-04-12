// See https://aka.ms/new-console-template for more information

namespace CadDev {
public static class Program
{
    public static void Main(string[] args)
    {
        CadDev cad = new CadDev(640, 640, "CadDev");
        cad.Run();
    }
}

}