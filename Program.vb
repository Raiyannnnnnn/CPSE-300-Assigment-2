Imports System
Imports System.Windows.Forms

Module Program
    <STAThread>
    Sub Main()
        ' Uncomment the next line to run console tests instead of GUI
        ' RunConsoleTests()
        
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        Application.Run(New Form1())
    End Sub

    Private Sub RunConsoleTests()
        Console.WriteLine("=== Running Console Tests ===")
        
        Dim customers As New List(Of Customer)()
        customers.Add(New Customer(0, 5))
        customers.Add(New Customer(2, 3))
        
        Dim simulation As New BankSimulation()
        Console.WriteLine(simulation.RunSimulation(customers))
        
        Console.WriteLine("Press any key to exit console mode...")
        Console.ReadKey()
    End Sub
End Module
