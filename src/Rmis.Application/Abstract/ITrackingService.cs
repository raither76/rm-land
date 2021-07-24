namespace Rmis.Application.Abstract
{
    public interface ITrackingService
    {
        void SaveTrackInfo(TrackInfoDto trackInfoDto);
        
        TrackInfoDto GetLastTrackInfo(string trainNumber);
    }
}