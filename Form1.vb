Imports System.IO
Imports System.Drawing

Public Class Form1
    Private simulation As BankSimulation
    Private customers As List(Of Customer)
    Private simulationResults As String
    Private analysisResults As String

    Public Sub New()
        InitializeComponent()
        simulation = New BankSimulation()
        customers = New List(Of Customer)()
        SetupInitialState()
    End Sub

    Private Sub SetupInitialState()
        btnRunSimulation.Enabled = False
        btnExport.Enabled = False
        lblStatus.Text = "Ready to load customer data"
        lblStatus.ForeColor = Color.Blue
        ProgressBar1.Visible = False
    End Sub

    Private Sub btnLoadFile_Click(sender As Object, e As EventArgs) Handles btnLoadFile.Click
        Using openFileDialog As New OpenFileDialog()
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
            openFileDialog.Title = "Select Customer Data File"
            openFileDialog.InitialDirectory = Application.StartupPath

            If openFileDialog.ShowDialog() = DialogResult.OK Then
                Try
                    Cursor = Cursors.WaitCursor
                    ProgressBar1.Visible = True
                    ProgressBar1.Style = ProgressBarStyle.Marquee

                    LoadCustomerData(openFileDialog.FileName)
                    lblStatus.Text = $"File loaded successfully! {customers.Count} customers loaded."
                    lblStatus.ForeColor = Color.Green

                    ' Update statistics
                    UpdateFileStatistics()

                Catch ex As Exception
                    lblStatus.Text = "Error loading file: " & ex.Message
                    lblStatus.ForeColor = Color.Red
                    MessageBox.Show("Error loading file: " & ex.Message, "File Error", 
                                  MessageBoxButtons.OK, MessageBoxIcon.Error)
                Finally
                    Cursor = Cursors.Default
                    ProgressBar1.Visible = False
                End Try
            End If
        End Using
    End Sub

    Private Sub LoadCustomerData(filePath As String)
        customers.Clear()
        lstCustomers.Items.Clear()

        Dim lines() As String = File.ReadAllLines(filePath)
        Dim lineNumber As Integer = 0
        Dim customerId As Integer = 1
        
        For Each line In lines
            lineNumber += 1
            If String.IsNullOrWhiteSpace(line) Then Continue For

            ' Parse space-separated values (based on your arrivals.txt format)
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

            If arrivalTime < 0 Then
                Throw New FormatException($"Arrival time cannot be negative at line {lineNumber}: {arrivalTime}")
            End If
            If serviceTime <= 0 Then
                Throw New FormatException($"Service time must be positive at line {lineNumber}: {serviceTime}")
            End If

            ' Create customer with auto-incrementing ID
            Dim customer As New Customer(arrivalTime, serviceTime)
            customers.Add(customer)
            
            ' Add to listbox with formatted display
            lstCustomers.Items.Add($"Customer {customerId}: Arrival={arrivalTime}, Service={serviceTime}")
            customerId += 1
        Next

        If customers.Count = 0 Then
            Throw New Exception("No valid customer data found in file.")
        End If

        btnRunSimulation.Enabled = True
    End Sub

    Private Sub UpdateFileStatistics()
        If customers.Count > 0 Then
            Dim firstArrival As Integer = customers.Min(Function(c) c.GetArrivalTime())
            Dim lastArrival As Integer = customers.Max(Function(c) c.GetArrivalTime())
            Dim totalService As Integer = customers.Sum(Function(c) c.GetServiceTime())
            Dim avgService As Double = customers.Average(Function(c) c.GetServiceTime())

            lblFileStats.Text = $"Customers: {customers.Count} | " &
                              $"Arrival Range: {firstArrival}-{lastArrival} | " &
                              $"Avg Service: {avgService:F1} | " &
                              $"Total Service: {totalService}"
        Else
            lblFileStats.Text = "No data loaded"
        End If
    End Sub

    Private Sub btnRunSimulation_Click(sender As Object, e As EventArgs) Handles btnRunSimulation.Click
        If customers.Count = 0 Then
            MessageBox.Show("No customer data loaded! Please load a data file first.", 
                          "No Data", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            Cursor = Cursors.WaitCursor
            ProgressBar1.Visible = True
            ProgressBar1.Style = ProgressBarStyle.Marquee

            ' Clear previous results
            txtSimulationOutput.Clear()
            txtAnalysis.Clear()

            ' Run simulation
            simulationResults = simulation.RunSimulation(customers)
            analysisResults = simulation.GetAnalysis()

            ' Display results
            DisplayResults()
            
            lblStatus.Text = "Simulation completed successfully!"
            lblStatus.ForeColor = Color.Green
            btnExport.Enabled = True

            ' Update simulation stats
            UpdateSimulationStats()

        Catch ex As Exception
            lblStatus.Text = "Simulation error: " & ex.Message
            lblStatus.ForeColor = Color.Red
            MessageBox.Show("Simulation error: " & ex.Message, "Simulation Error", 
                          MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Cursor = Cursors.Default
            ProgressBar1.Visible = False
        End Try
    End Sub

    Private Sub UpdateSimulationStats()
        If customers.Count > 0 AndAlso customers(0).GetDepartureTime() > 0 Then
            Dim lastDeparture As Integer = customers.Max(Function(c) c.GetDepartureTime())
            Dim totalWait As Integer = customers.Sum(Function(c) c.GetDepartureTime() - c.GetArrivalTime() - c.GetServiceTime())
            Dim avgWait As Double = totalWait / customers.Count

            lblSimStats.Text = $"Simulation Time: {lastDeparture} | " &
                             $"Total Wait: {totalWait} | " &
                             $"Avg Wait: {avgWait:F1}"
        Else
            lblSimStats.Text = "Run simulation to see statistics"
        End If
    End Sub

    Private Sub DisplayResults()
        ' Display simulation process
        txtSimulationOutput.Text = simulationResults

        ' Display analysis with highlighting
        DisplayAnalysisWithHighlighting()
    End Sub

    Private Sub DisplayAnalysisWithHighlighting()
        txtAnalysis.Clear()

        If String.IsNullOrEmpty(analysisResults) Then Exit Sub

        Dim analysisLines() As String = analysisResults.Split(New String() {Environment.NewLine}, StringSplitOptions.None)
        
        For Each line In analysisLines
            If line.Contains("Average") Or line.Contains("Maximum") Or line.Contains("Total") Then
                AppendColoredText(txtAnalysis, line, Color.Blue, True)
            ElseIf line.Contains("utilization") Or line.Contains("utilization") Then
                AppendColoredText(txtAnalysis, line, Color.DarkGreen, True)
            ElseIf line.Contains("===") Or line.Contains("---") Then
                AppendColoredText(txtAnalysis, line, Color.DarkRed, True)
            ElseIf line.StartsWith("SIMULATION ANALYSIS") Then
                AppendColoredText(txtAnalysis, line, Color.DarkBlue, True)
            ElseIf line.StartsWith("INDIVIDUAL") Or line.StartsWith("SUMMARY") Then
                AppendColoredText(txtAnalysis, line, Color.Purple, True)
            Else
                txtAnalysis.AppendText(line & Environment.NewLine)
            End If
        Next
    End Sub

    Private Sub AppendColoredText(textBox As TextBox, text As String, color As Color, Optional bold As Boolean = False)
        textBox.SelectionStart = textBox.TextLength
        textBox.SelectionLength = 0
        textBox.SelectionColor = color
        If bold Then
            textBox.SelectionFont = New Font(textBox.Font, FontStyle.Bold)
        End If
        textBox.AppendText(text & Environment.NewLine)
        textBox.SelectionColor = textBox.ForeColor
        textBox.SelectionFont = textBox.Font
    End Sub

    Private Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click
        If String.IsNullOrEmpty(simulationResults) OrElse String.IsNullOrEmpty(analysisResults) Then
            MessageBox.Show("No simulation results to export. Please run the simulation first.", 
                          "No Results", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Using saveFileDialog As New SaveFileDialog()
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
            saveFileDialog.Title = "Save Simulation Results"
            saveFileDialog.DefaultExt = "txt"
            saveFileDialog.FileName = $"BankSimulation_Results_{DateTime.Now:yyyyMMdd_HHmmss}.txt"

            If saveFileDialog.ShowDialog() = DialogResult.OK Then
                Try
                    Using writer As New StreamWriter(saveFileDialog.FileName)
                        writer.WriteLine("BANK LINE-UP SIMULATION RESULTS")
                        writer.WriteLine("=================================")
                        writer.WriteLine($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}")
                        writer.WriteLine($"Team: {GetTeamName()}")
                        writer.WriteLine()
                        writer.WriteLine("INPUT DATA SUMMARY:")
                        writer.WriteLine($"Total customers: {customers.Count}")
                        writer.WriteLine($"Arrival time range: {customers.Min(Function(c) c.GetArrivalTime())} - {customers.Max(Function(c) c.GetArrivalTime())}")
                        writer.WriteLine($"Total service time: {customers.Sum(Function(c) c.GetServiceTime())}")
                        writer.WriteLine()
                        writer.WriteLine("SIMULATION PROCESS:")
                        writer.WriteLine(New String("-"c, 50))
                        writer.WriteLine(simulationResults)
                        writer.WriteLine()
                        writer.WriteLine("ANALYSIS RESULTS:")
                        writer.WriteLine(New String("-"c, 50))
                        writer.WriteLine(analysisResults)
                    End Using

                    lblStatus.Text = $"Results exported to: {Path.GetFileName(saveFileDialog.FileName)}"
                    lblStatus.ForeColor = Color.DarkBlue
                    
                    MessageBox.Show($"Results successfully exported to:{Environment.NewLine}{saveFileDialog.FileName}", 
                                  "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)

                Catch ex As Exception
                    MessageBox.Show("Error exporting results: " & ex.Message, "Export Error", 
                                  MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If
        End Using
    End Sub

    Private Function GetTeamName() As String
        ' Update this with your actual team name
        Return "Your Team Name"
    End Function

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        customers.Clear()
        lstCustomers.Items.Clear()
        txtSimulationOutput.Clear()
        txtAnalysis.Clear()
        simulationResults = ""
        analysisResults = ""
        btnRunSimulation.Enabled = False
        btnExport.Enabled = False
        lblStatus.Text = "Ready to load customer data"
        lblStatus.ForeColor = Color.Blue
        lblFileStats.Text = "No file loaded"
        lblSimStats.Text = "No simulation run"
    End Sub

    Private Sub btnAbout_Click(sender As Object, e As EventArgs) Handles btnAbout.Click
        MessageBox.Show(
            "Bank Line-Up Simulation System" & Environment.NewLine &
            "Version 1.0" & Environment.NewLine &
            "CPSC 300 Assignment" & Environment.NewLine &
            Environment.NewLine &
            "Features:" & Environment.NewLine &
            "• Event-driven bank simulation" & Environment.NewLine &
            "• Customer queue management" & Environment.NewLine &
            "• Detailed statistics and analysis" & Environment.NewLine &
            "• Export results to file" & Environment.NewLine &
            Environment.NewLine &
            "File format: ArrivalTime ServiceTime (space-separated)",
            "About", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        lblStatus.Text = "Ready to load customer data"
        lblFileStats.Text = "No file loaded"
        lblSimStats.Text = "No simulation run"
    End Sub

    Private Sub lnkHelp_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lnkHelp.LinkClicked
        MessageBox.Show(
            "How to use this application:" & Environment.NewLine &
            Environment.NewLine &
            "1. Click 'Load Data File' to load customer data" & Environment.NewLine &
            "2. Data format: ArrivalTime ServiceTime (space-separated)" & Environment.NewLine &
            "3. Click 'Run Simulation' to start the simulation" & Environment.NewLine &
            "4. View results in the Simulation and Analysis tabs" & Environment.NewLine &
            "5. Export results using 'Export Results' button" & Environment.NewLine &
            Environment.NewLine &
            "Example data line: '18 2' (Arrival at time 18, Service time 2)",
            "Help", MessageBoxButtons.OK, MessageBoxIcon.Question)
    End Sub
End Class