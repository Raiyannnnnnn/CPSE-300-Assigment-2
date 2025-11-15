Imports System
Imports System.Collections.Generic

Module Program
    Sub Main(args As String())
        ' ====== Create sample customers ======
        Dim customers As New List(Of Customer)()
        customers.Add(New Customer(0, 5))   ' Arrives at time 0, needs 5 units of service
        customers.Add(New Customer(2, 3))   ' Arrives at time 2, needs 3 units of service
        customers.Add(New Customer(4, 2))   ' Arrives at time 4, needs 2 units of service
        customers.Add(New Customer(6, 4))   ' Arrives at time 6, needs 4 units of service

        ' ====== Create and run simulation ======
        Dim simulation As New BankSimulation()
        simulation.Initialize(customers)
        simulation.Run()

        ' ====== Display results ======
        Console.WriteLine(simulation.GetFormattedOutput())

        Console.WriteLine(vbCrLf & "Press any key to exit...")
        Console.ReadKey()
    End Sub
End Module
