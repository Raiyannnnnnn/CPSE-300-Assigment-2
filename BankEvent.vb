' BankEvent.vb
' Represents an arrival or departure event for a specific customer

Public Class BankEvent
    Implements IComparable(Of BankEvent)

    ' ====== Fields ======
    Private ReadOnly eventTime As Integer        ' When this event happens
    Private ReadOnly customer As Customer        ' The customer this event is for
    Private ReadOnly eventType As String         ' "ARRIVAL" or "DEPARTURE"

    ' ====== Constructor ======
    Public Sub New(typeName As String, timeValue As Integer, cust As Customer)
        eventType = typeName
        eventTime = timeValue
        customer = cust
    End Sub

    ' ====== Getters ======
    Public Function GetTypeName() As String
        Return eventType
    End Function

    Public Function GetTime() As Integer
        Return eventTime
    End Function

    Public Function GetCustomer() As Customer
        Return customer
    End Function

    ' ====== Compare events by their time ======
    Public Function CompareTo(other As BankEvent) As Integer Implements IComparable(Of BankEvent).CompareTo
        If other Is Nothing Then Return 1

        If Me.eventTime > other.eventTime Then
            Return 1
        ElseIf Me.eventTime < other.eventTime Then
            Return -1
        Else
            Return 0
        End If
    End Function

    ' ====== String representation ======
    Public Overrides Function ToString() As String
        Return $"{eventType} event at time {eventTime} for customer (arrival: {customer.GetArrivalTime()}, service: {customer.GetServiceTime()})"
    End Function

End Class
