namespace PskOnline.X20.Protocol.Internal
{
  public interface ICommand
  {
    ICommandResponse Execute(IUsbControlPipe controlPipe);
  }
}
