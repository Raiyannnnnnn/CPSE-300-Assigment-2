' BankSimulation.vb
' Implements the event-driven bank queue simulation algorithm

Imports System.Collections.Generic
Imports System.Text

Public Class BankSimulation

    ' ====== Fields ======
    Private eventQueue As BankEventQueue          ' Priority queue for events
    Private customerQueue As CustomerQueue        ' FIFO queue for waiting customers
    Private allCustomers As List(Of Customer)     ' All customers in the simulation
    Private currentTime As Integer                ' Current simulation time
    Private tellerBusy As Boolean                 ' Whether teller is currently serving a customer
    Private currentCustomer As Customer           ' Customer currently being served (if any)

    ' ====== Statistics ======
    Private totalWaitTime As Integer              ' Sum of all wait times
    Private totalCustomers As Integer             ' Total number of customers
    Private maxQueueLength As Integer             ' Maximum queue length observed
    Private simulationLog As List(Of String)      ' Log of simulation events

    ' ====== Constructor ======
    Public Sub New()
        eventQueue = New BankEventQueue()
        customerQueue = New CustomerQueue()
        allCustomers = New List(Of Customer)()
        currentTime = 0
        tellerBusy = False
        currentCustomer = Nothing
        totalWaitTime = 0
        totalCustomers = 0
        maxQueueLength = 0
        simulationLog = New List(Of String)()
    End Sub

    ' ====== Initialize simulation with a list of customers ======
    Public Sub Initialize(customers As List(Of Customer))
        ' Clear previous state
        eventQueue.Clear()
        customerQueue.Clear()
        allCustomers.Clear()
        simulationLog.Clear()
        currentTime = 0
        tellerBusy = False
        currentCustomer = Nothing
        totalWaitTime = 0
        totalCustomers = 0
        maxQueueLength = 0

        ' Store all customers
        allCustomers = customers
        totalCustomers = customers.Count

        ' Create arrival events for all customers
        For Each cust As Customer In customers
            Dim arrivalEvent As New BankEvent("ARRIVAL", cust.GetArrivalTime(), cust)
            eventQueue.Add(arrivalEvent)
        Next

        simulationLog.Add("=== Bank Simulation Initialized ===")
        simulationLog.Add($"Total customers: {totalCustomers}")
        simulationLog.Add("")
    End Sub

    ' ====== Run the event-driven simulation ======
    Public Sub Run()
        simulationLog.Add("=== Simulation Started ===")
        simulationLog.Add("")

        ' Process events until event queue is empty
        While Not eventQueue.IsEmpty()
            ' Get the next event (earliest time)
            Dim currentEvent As BankEvent = eventQueue.RemoveNext()
            currentTime = currentEvent.GetTime()
            Dim customer As Customer = currentEvent.GetCustomer()

            ' Process based on event type
            If currentEvent.GetTypeName() = "ARRIVAL" Then
                ProcessArrival(customer)
            ElseIf currentEvent.GetTypeName() = "DEPARTURE" Then
                ProcessDeparture(customer)
            End If

            ' Update max queue length
            If customerQueue.GetSize() > maxQueueLength Then
                maxQueueLength = customerQueue.GetSize()
            End If
        End While

        simulationLog.Add("=== Simulation Completed ===")
        simulationLog.Add("")
    End Sub

    ' ====== Process an arrival event ======
    Private Sub ProcessArrival(customer As Customer)
        simulationLog.Add($"Time {currentTime}: Customer arrives (arrival={customer.GetArrivalTime()}, service={customer.GetServiceTime()})")

        If tellerBusy Then
            ' Teller is busy, customer joins the queue
            customerQueue.Enqueue(customer)
            simulationLog.Add($"  -> Customer joins queue. Queue length: {customerQueue.GetSize()}")
        Else
            ' Teller is free, start serving immediately
            tellerBusy = True
            currentCustomer = customer
            Dim departureTime As Integer = currentTime + customer.GetServiceTime()
            customer.SetDepartureTime(departureTime)
            
            ' Create departure event
            Dim departureEvent As New BankEvent("DEPARTURE", departureTime, customer)
            eventQueue.Add(departureEvent)
            
            simulationLog.Add($"  -> Customer starts service immediately. Will depart at time {departureTime}")
        End If
    End Sub

    ' ====== Process a departure event ======
    Private Sub ProcessDeparture(customer As Customer)
        Dim waitTime As Integer = currentTime - customer.GetArrivalTime() - customer.GetServiceTime()
        totalWaitTime += waitTime

        simulationLog.Add($"Time {currentTime}: Customer departs (arrival={customer.GetArrivalTime()}, service={customer.GetServiceTime()}, wait={waitTime})")

        ' Customer is done, teller becomes free
        tellerBusy = False
        currentCustomer = Nothing

        ' Check if there are customers waiting in queue
        If Not customerQueue.IsEmpty() Then
            ' Get next customer from queue
            Dim nextCustomer As Customer = customerQueue.Dequeue()
            tellerBusy = True
            currentCustomer = nextCustomer

            ' Calculate wait time for this customer
            Dim customerWaitTime As Integer = currentTime - nextCustomer.GetArrivalTime()
            totalWaitTime += customerWaitTime

            ' Calculate departure time
            Dim departureTime As Integer = currentTime + nextCustomer.GetServiceTime()
            nextCustomer.SetDepartureTime(departureTime)

            ' Create departure event
            Dim departureEvent As New BankEvent("DEPARTURE", departureTime, nextCustomer)
            eventQueue.Add(departureEvent)

            simulationLog.Add($"  -> Next customer starts service (waited {customerWaitTime} units). Will depart at time {departureTime}")
        Else
            simulationLog.Add($"  -> No customers waiting. Teller becomes idle.")
        End If
    End Sub

    ' ====== Get average wait time ======
    Public Function GetAverageWaitTime() As Double
        If totalCustomers = 0 Then Return 0.0
        Return totalWaitTime / totalCustomers
    End Function

    ' ====== Get maximum queue length ======
    Public Function GetMaxQueueLength() As Integer
        Return maxQueueLength
    End Function

    ' ====== Get total customers ======
    Public Function GetTotalCustomers() As Integer
        Return totalCustomers
    End Function

    ' ====== Get total wait time ======
    Public Function GetTotalWaitTime() As Integer
        Return totalWaitTime
    End Function

    ' ====== Get simulation log ======
    Public Function GetSimulationLog() As List(Of String)
        Return simulationLog
    End Function

    ' ====== Get formatted output for display ======
    Public Function GetFormattedOutput() As String
        Dim output As New StringBuilder()

        ' Detailed simulation log
        output.AppendLine("=== DETAILED SIMULATION LOG ===")
        output.AppendLine()
        For Each logEntry As String In simulationLog
            output.AppendLine(logEntry)
        Next
        output.AppendLine()

        ' Summary statistics
        output.AppendLine("=== SIMULATION ANALYSIS ===")
        output.AppendLine()
        output.AppendLine($"Total Customers Processed: {totalCustomers}")
        output.AppendLine($"Total Wait Time: {totalWaitTime} time units")
        output.AppendLine($"Average Wait Time: {GetAverageWaitTime():F2} time units")
        output.AppendLine($"Maximum Queue Length: {maxQueueLength}")
        output.AppendLine()

        ' Individual customer details
        output.AppendLine("=== CUSTOMER DETAILS ===")
        output.AppendLine()
        output.AppendLine("Customer | Arrival | Service | Departure | Wait Time | Total Time")
        output.AppendLine("---------|---------|---------|-----------|-----------|-----------")
        For Each cust As Customer In allCustomers
            Dim waitTime As Integer = cust.GetDepartureTime() - cust.GetArrivalTime() - cust.GetServiceTime()
            Dim totalTime As Integer = cust.TotalTime()
            output.AppendLine($"{allCustomers.IndexOf(cust) + 1,8} | {cust.GetArrivalTime(),7} | {cust.GetServiceTime(),7} | {cust.GetDepartureTime(),9} | {waitTime,9} | {totalTime,10}")
        Next

        Return output.ToString()
    End Function

End Class
