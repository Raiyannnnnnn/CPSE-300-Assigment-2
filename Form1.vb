Imports System.IO
Imports System.Drawing

Public Class Form1
    Private simulation As BankSimulation
    Private customers As List(Of Customer)
    Private simulationResults As String
    Private analysisResults As String

    Private WithEvents btnLoadFile As Button
    Private WithEvents btnRunSimulation As Button
    Private WithEvents btnExport As Button
    Private WithEvents btnClear As Button
    Private lstCustomers As ListBox
    Private txtOutput As TextBox
    Private lblStatus As Label

    Public Sub New()
        InitializeComponent()
        CreateControls()
        simulation = New BankSimulation()
        customers = New List(Of Customer)()
    End Sub

    Private Sub CreateControls()
        ' Set form properties
        Me.Text = "Bank Simulation System"
        Me.Size = New Size(800, 600)
        Me.StartPosition = FormStartPosition.CenterScreen

        ' Create and configure controls
        btnLoadFile = New Button With {
            .Text = "Load Data File",
            .Location = New Point(20, 20),
            .Size = New Size(120, 35)
        }

        btnRunSimulation = New Button With {
            .Text = "Run Simulation",
            .Location = New Point(150, 20),
            .Size = New Size(120, 35)
        }

        btnExport = New Button With {
            .Text = "Export Results",
            .Location = New Point(280, 20),
            .Size = New Size(120, 35)
        }

        btnClear = New Button With {
            .Text = "Clear All",
            .Location = New Point(410, 20),
            .Size = New Size(120, 35)
        }

        lstCustomers = New ListBox With {
            .Location = New Point(20, 70),
            .Size = New Size(250, 400)
        }

        txtOutput = New TextBox With {
            .Location = New Point(280, 70),
            .Size = New Size(480, 400),
            .Multiline = True,
            .ScrollBars = ScrollBars.Both,
            .Font = New Font("Consolas", 9)
        }

        lblStatus = New Label With {
            .Text = "Ready to load customer data",
            .Location = New Point(20, 480),
            .Size = New Size(500, 20),
            .ForeColor = Color.Blue
        }

        ' Add controls to form
        Me.Controls.Add(btnLoadFile)
        Me.Controls.Add(btnRunSimulation)
        Me.Controls.Add(btnExport)
        Me.Controls.Add(btnClear)
        Me.Controls.Add(lstCustomers)
        Me.Controls.Add(txtOutput)
        Me.Controls.Add(lblStatus)

        ' Set initial state
        btnRunSimulation.Enabled = False
        btnExport.Enabled = False
    End Sub

    Private Sub btnLoadFile_Click(sender As Object, e As EventArgs) Handles btnLoadFile.Click
        Using openFileDialog As New OpenFileDialog()
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
            openFileDialog.Title = "Select Customer Data File"
            openFileDialog.InitialDirectory = Application.StartupPath

            If openFileDialog.ShowDialog() = DialogResult.OK Then
                Try
                    LoadCustomerData(openFileDialog.FileName)
                    lblStatus.Text = $"File loaded successfully! {customers.Count} customers loaded."
                    lblStatus.ForeColor = Color.Green
                Catch ex As Exception
                    lblStatus.Text = "Error loading file: " & ex.Message
                    lblStatus.ForeColor = Color.Red
                    MessageBox.Show("Error loading file: " & ex.Message, "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If
        End Using
    End Sub

    Private Sub LoadCustomerData(filePath As String)
        customers.Clear()
        lstCustomers.Items.Clear()

        Dim lines() As String = File.ReadAllLines(filePath)
        Dim lineNumber As Integer = 0

        For Each line In lines
            lineNumber += 1
            If String.IsNullOrWhiteSpace(line) Then Continue For

            Dim parts() As String = line.Split(" "c)
            If parts.Length < 2 Then
                Throw New FormatException($"Invalid line format at line {lineNumber}: {line}. Expected: ArrivalTime ServiceTime")
            End If

            Dim arrivalTime As Integer
            Dim serviceTime As Integer

            If Not Integer.TryParse(parts(0).Trim(), arrivalTime) Then
                Throw New FormatException($"Invalid arrival time at line {lineNumber}: {parts(0)}")
            End If
            If Not Integer.TryParse(parts(1).Trim(), serviceTime) Then
                Throw New FormatException($"Invalid service time at line {lineNumber}: {parts(1)}")
            End If

            Dim customer As New Customer(arrivalTime, serviceTime)
            customers.Add(customer)
            lstCustomers.Items.Add($"Customer {customers.Count}: Arrival={arrivalTime}, Service={serviceTime}")
        Next

        btnRunSimulation.Enabled = customers.Count > 0
    End Sub

    Private Sub btnRunSimulation_Click(sender As Object, e As EventArgs) Handles btnRunSimulation.Click
        If customers.Count = 0 Then
            MessageBox.Show("No customer data loaded! Please load a data file first.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            simulationResults = simulation.RunSimulation(customers)
            analysisResults = simulation.GetAnalysis()

            txtOutput.Text = "=== SIMULATION PROCESS ===" & Environment.NewLine &
                           simulationResults & Environment.NewLine & Environment.NewLine &
                           "=== ANALYSIS RESULTS ===" & Environment.NewLine &
                           analysisResults

            btnExport.Enabled = True
            lblStatus.Text = "Simulation completed successfully!"
            lblStatus.ForeColor = Color.Green

        Catch ex As Exception
            lblStatus.Text = "Simulation error: " & ex.Message
            lblStatus.ForeColor = Color.Red
            MessageBox.Show("Simulation error: " & ex.Message, "Simulation Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click
        If String.IsNullOrEmpty(txtOutput.Text) Then
            MessageBox.Show("No results to export. Please run the simulation first.", "No Results", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Using saveFileDialog As New SaveFileDialog()
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
            saveFileDialog.Title = "Save Simulation Results"
            saveFileDialog.DefaultExt = "txt"
            saveFileDialog.FileName = $"BankSimulation_Results_{DateTime.Now:yyyyMMdd_HHmmss}.txt"

            If saveFileDialog.ShowDialog() = DialogResult.OK Then
                Try
                    File.WriteAllText(saveFileDialog.FileName, txtOutput.Text)
                    MessageBox.Show($"Results successfully exported to:{Environment.NewLine}{saveFileDialog.FileName}", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Catch ex As Exception
                    MessageBox.Show("Error exporting results: " & ex.Message, "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If
        End Using
    End Sub

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        customers.Clear()
        lstCustomers.Items.Clear()
        txtOutput.Clear()
        simulationResults = ""
        analysisResults = ""
        btnRunSimulation.Enabled = False
        btnExport.Enabled = False
        lblStatus.Text = "Ready to load customer data"
        lblStatus.ForeColor = Color.Blue
    End Sub
End Class
