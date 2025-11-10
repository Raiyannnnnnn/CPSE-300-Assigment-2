' BankEventQueue.vb
' A priority queue for BankEvent objects (earliest event first)

Public Class BankEventQueue
    Private front As Node   ' Points to the earliest event
    Private size As Integer ' Number of events currently in the queue

    ' ====== Inner Node class ======
    Private Class Node
        Public Data As BankEvent
        Public NextNode As Node

        Public Sub New(e As BankEvent)
            Data = e
            NextNode = Nothing
        End Sub
    End Class

    ' ====== Constructor ======
    Public Sub New()
        front = Nothing
        size = 0
    End Sub

    ' ====== Check if queue is empty ======
    Public Function IsEmpty() As Boolean
        Return size = 0
    End Function

    ' ====== Return number of events ======
    Public Function GetSize() As Integer
        Return size
    End Function

    ' ====== Add a new event in sorted order (earliest time first) ======
    Public Sub Add(e As BankEvent)
        Dim newNode As New Node(e)

        ' If queue is empty OR event time is earlier than front
        If IsEmpty() OrElse e.GetTime() < front.Data.GetTime() Then
            newNode.NextNode = front
            front = newNode
        Else
            ' Find the right spot to insert
            Dim current As Node = front
            While current.NextNode IsNot Nothing AndAlso current.NextNode.Data.GetTime() <= e.GetTime()
                current = current.NextNode
            End While

            ' Insert after current
            newNode.NextNode = current.NextNode
            current.NextNode = newNode
        End If

        size += 1
    End Sub

    ' ====== Peek at earliest event (without removing) ======
    Public Function Peek() As BankEvent
        If IsEmpty() Then
            Return Nothing
        Else
            Return front.Data
        End If
    End Function

    ' ====== Remove and return earliest event ======
    Public Function RemoveNext() As BankEvent
        If IsEmpty() Then
            Return Nothing
        End If

        Dim result As BankEvent = front.Data
        front = front.NextNode
        size -= 1
        Return result
    End Function

    ' ====== Clear all events ======
    Public Sub Clear()
        front = Nothing
        size = 0
    End Sub

    ' ====== Debugging string output ======
    Public Overrides Function ToString() As String
        If IsEmpty() Then
            Return "[empty]"
        End If

        Dim sb As New Text.StringBuilder()
        Dim current As Node = front
        While current IsNot Nothing
            sb.Append(current.Data.ToString() & " -> ")
            current = current.NextNode
        End While
        sb.Append("null")
        Return sb.ToString()
    End Function

End Class
