Imports System

Module Program
    Sub Main(args As String())
        ' ====== Create customers ======
        Dim c1 As New Customer(0, 5)
        Dim c2 As New Customer(2, 3)

        ' ====== Create events ======
        Dim e1 As New BankEvent("ARRIVAL", 0, c1)
        Dim e2 As New BankEvent("ARRIVAL", 2, c2)
        Dim e3 As New BankEvent("DEPARTURE", 7, c1)

        ' ====== Create and test BankEventQueue ======
        Dim eq As New BankEventQueue()
        eq.Add(e2)
        eq.Add(e1)
        eq.Add(e3)

        Console.WriteLine("=== BankEventQueue Contents ===")
        Console.WriteLine(eq.ToString())

        ' ====== Test removing the earliest event ======
        Console.WriteLine(vbCrLf & "Removing earliest event...")
        Dim firstEvent As BankEvent = eq.RemoveNext()
        Console.WriteLine("Removed: " & firstEvent.ToString())

        ' ====== Show remaining events ======
        Console.WriteLine(vbCrLf & "Remaining events:")
        Console.WriteLine(eq.ToString())

        Console.WriteLine(vbCrLf & "Press any key to exit...")
        Console.ReadKey()
    End Sub
End Module
