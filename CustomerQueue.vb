' CustomerQueue.vb
' A singly linked queue for Customer objects (FIFO order)

Public Class CustomerQueue

    ' ====== Inner Node class ======
    Private Class Node
        Public Data As Customer
        Public [NextNode] As Node

        Public Sub New(c As Customer)
            Data = c
            [NextNode] = Nothing
        End Sub
    End Class

    ' ====== Fields ======
    Private front As Node   ' Head of the queue
    Private rear As Node    ' Tail of the queue
    Private size As Integer ' Element count

    ' ====== Constructor ======
    Public Sub New()
        front = Nothing
        rear = Nothing
        size = 0
    End Sub

    ' ====== Check if queue is empty ======
    Public Function IsEmpty() As Boolean
        Return size = 0
    End Function

    ' ====== Return number of elements ======
    Public Function GetSize() As Integer
        Return size
    End Function

    ' ====== Add a new customer to the rear ======
    Public Sub Enqueue(c As Customer)
        Dim newNode As New Node(c)

        If IsEmpty() Then
            front = newNode
            rear = newNode
        Else
            rear.NextNode = newNode
            rear = newNode
        End If

        size += 1
    End Sub

    ' ====== Remove and return the front customer ======
    Public Function Dequeue() As Customer
        If IsEmpty() Then
            Return Nothing
        End If

        Dim result As Customer = front.Data
        front = front.NextNode

        ' If queue becomes empty, set rear to Nothing
        If front Is Nothing Then
            rear = Nothing
        End If

        size -= 1
        Return result
    End Function

    ' ====== Peek at the front customer ======
    Public Function Peek() As Customer
        If IsEmpty() Then
            Return Nothing
        Else
            Return front.Data
        End If
    End Function

    ' ====== Clear the queue ======
    Public Sub Clear()
        front = Nothing
        rear = Nothing
        size = 0
    End Sub

    ' ====== String representation for debugging ======
    Public Overrides Function ToString() As String
        If IsEmpty() Then
            Return "[empty]"
        End If

        Dim sb As New Text.StringBuilder()
        Dim current As Node = front

        While current IsNot Nothing
            sb.Append($"Customer(arrival={current.Data.GetArrivalTime()}, service={current.Data.GetServiceTime()}) -> ")
            current = current.NextNode
        End While

        sb.Append("null")
        Return sb.ToString()
    End Function

End Class
