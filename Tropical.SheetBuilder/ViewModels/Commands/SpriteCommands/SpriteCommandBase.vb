''' <summary>
''' A base class for simplified implementation of
''' <see cref="ICommand" /> as it pertains
''' specifically to sprites.
''' </summary>
Public MustInherit Class SpriteCommandBase
  Inherits CommandBase(Of Models.Sprite)

  ''' <summary>
  ''' Initializes a new instance of the <see cref="SpriteCommandBase"/> class.
  ''' </summary>
  ''' <param name="service">The sprite sheet service.</param>
  ''' <param name="canExecute">The function to be invoked as necessary
  ''' by <see cref="CanExecute" />.</param>
  ''' <param name="onExecuted">The method to invoke after a command
  ''' has been successfully executed. The affected sprite will be
  ''' passed to this method.</param>
  Public Sub New(service As SpriteSheetService,
                 canExecute As Func(Of Boolean),
                 onExecuted As Action(Of Models.Sprite))
    MyBase.New(service, canExecute, onExecuted)
  End Sub

End Class
