Class MainWindow 

  ''' <summary>
  ''' Changes the current sprite sheet to use the new provided
  ''' sheet as this instance's data context.
  ''' </summary>
  ''' <param name="sheet">The new sprite sheet.</param>
  Public Sub ChangeCurrentSpriteSheet(sheet As Models.SpriteSheet)

    ' Remove our handler from the previous data context
    Dim prevDataContext As SpriteSheetViewModel =
      TryCast(Me.DataContext, SpriteSheetViewModel)

    If prevDataContext IsNot Nothing Then
      RemoveHandler prevDataContext.NewSpriteSheetLoaded, AddressOf ChangeCurrentSpriteSheet
    End If

    ' Update our data context
    Dim newDataContext As New SpriteSheetViewModel(sheet, Me)
    Me.DataContext = newDataContext

    ' Add a handler for changing this again, if need be
    AddHandler newDataContext.NewSpriteSheetLoaded, AddressOf ChangeCurrentSpriteSheet

  End Sub

  Private Sub mnuExit_Click(sender As Object, e As RoutedEventArgs) Handles mnuExit.Click
    Me.Close()
  End Sub

  Private Sub mnuAbout_Click(sender As Object, e As RoutedEventArgs) Handles mnuAbout.Click

    Dim aboutText As New System.Text.StringBuilder()
    aboutText.AppendLine("Tropical Sprite Sheet Builder")
    aboutText.AppendLine("Version " & My.Application.Info.Version.ToString())
    aboutText.AppendLine()
    aboutText.AppendLine(My.Application.Info.Copyright)

    MessageBox.Show(aboutText.ToString(), "About", MessageBoxButton.OK, MessageBoxImage.Information)

  End Sub

End Class
