Class MainWindow

  ''' <summary>
  ''' Stores the default tooltips to be used for each textbox when
  ''' there are no errors.  Modified and accessed by 
  ''' <see cref="OnTextBoxValidationError" />.
  ''' </summary>
  Private ReadOnly DefaultTextBoxTooltips As New Dictionary(Of TextBox, String)

  ''' <summary>
  ''' Changes the current sprite sheet to use the new provided
  ''' sheet as this instance's data context.
  ''' </summary>
  ''' <param name="sheetInformation">The information about the new sprite sheet.</param>
  Public Sub ChangeCurrentSpriteSheet(sheetInformation As SpriteSheetFileInformation)

    ' Remove our handler from the previous data context
    Dim prevDataContext As SpriteSheetViewModel =
      TryCast(Me.DataContext, SpriteSheetViewModel)

    If prevDataContext IsNot Nothing Then
      RemoveHandler prevDataContext.NewSpriteSheetLoaded, AddressOf ChangeCurrentSpriteSheet
    End If

    ' Update our data context.
    Dim newDataContext As New SpriteSheetViewModel(sheetInformation.SpriteSheet, Me)

    ' Ensure the file name is set properly.
    If Not String.IsNullOrWhiteSpace(sheetInformation.FilePath) Then
      newDataContext.FileName = sheetInformation.FileName
    End If

    Me.DataContext = newDataContext

    ' Add a handler for changing this again, if need be
    AddHandler newDataContext.NewSpriteSheetLoaded, AddressOf ChangeCurrentSpriteSheet

  End Sub

  Private Sub CommitFocusedChanges(sender As Object, e As RoutedEventArgs)

    ' See if we have a textbox focused
    Dim focusedTextBox As TextBox = TryCast(FocusManager.GetFocusedElement(Me), TextBox)

    If focusedTextBox IsNot Nothing Then

      ' Get the binding expression for the Text property
      Dim textBindingEx As BindingExpression =
        focusedTextBox.GetBindingExpression(TextBox.TextProperty)

      ' Update the source if we found it
      If textBindingEx IsNot Nothing Then
        textBindingEx.UpdateSource()
      End If

    End If

  End Sub

  ''' <summary>
  ''' Called when a validation error has been added or removed for a textbox.
  ''' </summary>
  ''' <param name="sender">The sender.</param>
  ''' <param name="e">The <see cref="ValidationErrorEventArgs"/> instance containing the event data.</param>
  ''' <remarks>
  ''' This is a workaround for the fact that WPF will always prefer the locally-defined
  ''' property on an element.  However, when validation errors occur, we want to give
  ''' those precedence over the locally-defined tooltips.  This method will override
  ''' the tooltip to display when a validation error has taken place, and will revert
  ''' back to the default (as stored in <see cref="DefaultTextBoxTooltips" />) when
  ''' there are no longer any errors.
  ''' </remarks>
  Private Sub OnTextBoxValidationError(sender As Object, e As ValidationErrorEventArgs)

    Dim sourceTextBox As TextBox = TryCast(sender, TextBox)

    If sourceTextBox Is Nothing Then
      Return
    End If

    If e.Action = ValidationErrorEventAction.Added Then

      ' Store our default tooltip for the textbox
      If Not Me.DefaultTextBoxTooltips.ContainsKey(sourceTextBox) Then
        Me.DefaultTextBoxTooltips(sourceTextBox) = sourceTextBox.ToolTip.ToString()
      End If

      ' HACK: workaround format exceptions specially
      If e.Error.Exception Is Nothing OrElse Not TypeOf e.Error.Exception Is FormatException Then

        ' Set our tooltip to the error message
        sourceTextBox.SetValue(TextBox.ToolTipProperty, e.Error.ErrorContent)

      ElseIf TypeOf CType(e.Error.BindingInError, BindingExpression).ParentBinding.Converter Is IntConverter Then

        sourceTextBox.SetValue(TextBox.ToolTipProperty, "A positive number must be specified.")

      End If

    ElseIf Not Validation.GetHasError(sourceTextBox) AndAlso
      Me.DefaultTextBoxTooltips.ContainsKey(sourceTextBox) Then

      ' If we don't have any validation errors, use the default tooltip
      sourceTextBox.SetValue(TextBox.ToolTipProperty, Me.DefaultTextBoxTooltips(sourceTextBox))

    End If

  End Sub

  Private Sub MainWindow_Closing(sender As Object, e As ComponentModel.CancelEventArgs) Handles Me.Closing

    ' Ensure all changes have been committed
    CommitFocusedChanges(sender, New RoutedEventArgs())

    ' Cancel the close if we have unsaved changes that the user wants to keep.
    If Not CType(Me.DataContext, SpriteSheetViewModel).CheckUnsavedChangesHandler.Invoke() Then
      e.Cancel = True
    End If

  End Sub

  Private Sub mnuExit_Click(sender As Object, e As RoutedEventArgs)
    Me.Close()
  End Sub

  Private Sub mnuAbout_Click(sender As Object, e As RoutedEventArgs)

    Dim aboutText As New System.Text.StringBuilder()
    aboutText.AppendLine("Tropical Sprite Sheet Builder")
    aboutText.AppendLine("Version " & My.Application.Info.Version.ToString())
    aboutText.AppendLine()
    aboutText.AppendLine(My.Application.Info.Copyright)

    MessageBox.Show(aboutText.ToString(), "About", MessageBoxButton.OK, MessageBoxImage.Information)

  End Sub

End Class
