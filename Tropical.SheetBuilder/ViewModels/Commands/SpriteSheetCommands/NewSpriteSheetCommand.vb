Imports Tropical.Models

''' <summary>
''' A command for creating a new sprite sheet.
''' </summary>
Public Class NewSpriteSheetCommand
  Inherits CommandBase(Of SpriteSheet)
  Implements ICommand

  Private _lastPath As String

  ''' <summary>
  ''' Initializes a new instance of the <see cref="NewSpriteSheetCommand"/> class.
  ''' </summary>
  ''' <param name="service">The sprite sheet service.</param>
  ''' <param name="canExecute">The function to be invoked as necessary
  ''' by <see cref="CanExecute" />.</param>
  ''' <param name="created">The method to invoke after a sprite sheet
  ''' has been successfully created. The new sprite sheet will be
  ''' passed to this method.</param>
  Public Sub New(service As SpriteSheetService,
                 canExecute As Func(Of Boolean),
                 created As Action(Of SpriteSheet))

    MyBase.New(service, canExecute, created)

  End Sub

  ''' <summary>
  ''' Creates a new sprite sheet.
  ''' </summary>
  ''' <param name="parameter">This parameter is ignored.</param>
  Public Overrides Sub Execute(parameter As Object)

    If CanExecute(parameter) Then

      ' Create a new sheet.
      Dim sheet As New Models.SpriteSheet()

      ' Pass it to our handler.
      MyBase.TryExecuteHandler(sheet)

    End If

  End Sub

End Class
