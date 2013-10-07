''' <summary>
''' A base class for simplified implementation of
''' <see cref="ICommand" />.
''' </summary>
Public MustInherit Class SpriteCommandBase
  Implements ICommand

  Protected ReadOnly _service As SpriteSheetService
  Protected ReadOnly _canExecute As Func(Of Boolean)
  Protected ReadOnly _onExecuted As Action(Of Models.Sprite)

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
    _service = service
    _canExecute = canExecute
    _onExecuted = onExecuted
  End Sub

  ''' <summary>
  ''' Defines the method that determines whether the command can execute in its current state.
  ''' </summary>
  ''' <param name="parameter">Data used by the command.  
  ''' If the command does not require data to be passed, this object can be set to <c>null</c>.</param>
  ''' <returns>
  ''' <c>true</c> if this command can be executed; otherwise, <c>false</c>.
  ''' </returns>
  Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
    Return _canExecute()
  End Function

  ''' <summary>
  ''' Occurs when changes occur that affect whether or not the command should execute.
  ''' </summary>
  Public Event CanExecuteChanged(sender As Object, e As EventArgs) Implements ICommand.CanExecuteChanged

  ''' <summary>
  ''' Defines the method to be called when the command is invoked.
  ''' </summary>
  ''' <param name="parameter">Data used by the command.  
  ''' If the command does not require data to be passed, this object can be set to <c>null</c>.</param>
  Public MustOverride Sub Execute(parameter As Object) Implements ICommand.Execute

  ''' <summary>
  ''' Raises the <see cref="CanExecuteChanged" /> event.
  ''' </summary>
  Public Sub RaiseCanExecuteChanged()
    RaiseEvent CanExecuteChanged(Me, EventArgs.Empty)
  End Sub

End Class
