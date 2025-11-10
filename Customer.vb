' Customer.vb
' Represents a waiting bank customer

Public Class Customer

    ' ====== Fields ======
    Private arrivalTime As Integer
    Private departureTime As Integer
    Private serviceTime As Integer

    ' ====== Constructor ======
    Public Sub New(arrives As Integer, service As Integer)
        arrivalTime = arrives
        serviceTime = service
        departureTime = 0
    End Sub

    ' ====== Getters and Setters ======
    Public Function GetArrivalTime() As Integer
        Return arrivalTime
    End Function

    Public Function GetServiceTime() As Integer
        Return serviceTime
    End Function

    Public Sub SetDepartureTime(departs As Integer)
        departureTime = departs
    End Sub

    Public Function GetDepartureTime() As Integer
        Return departureTime
    End Function

    ' ====== Utility Method ======
    Public Function TotalTime() As Integer
        Return departureTime - arrivalTime
    End Function

    ' ====== Optional (for debugging) ======
    Public Overrides Function ToString() As String
        Return $"Customer(arrival={arrivalTime}, service={serviceTime}, departure={departureTime})"
    End Function

End Class
