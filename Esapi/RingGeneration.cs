using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using VMS.TPS.Common.Model.API;

namespace AutoRingUAB
{
    public class RingGeneration
    {
        public void CreateRingFromPTV(StructureSet structureSet, ObservableCollection<string> ptvIds, string ringInnerId, string ringMiddleId, string ringOuterId)
        {
            structureSet.Patient.BeginModifications();
            Structure ptvTotal = structureSet.AddStructure("Control", "zPTVtemp");
            Structure ringInner;
            Structure ringMiddle;
            Structure ringOuter;

            foreach (var ptvId in ptvIds)
            {
                Structure ptv = structureSet.Structures.Where(x => x.Id == ptvId).FirstOrDefault();
                ptvTotal.SegmentVolume = ptvTotal.Or(ptv.SegmentVolume);
            }

            //margins in mm
            double innerRingMargin;  
            double middleRingMargin;
            double outerRingMargin;
            if (ptvTotal.Volume <= 0.5)  //very small
            {
                innerRingMargin = 2;
                middleRingMargin = 5;
                outerRingMargin = 15;
            }
            else  //normal or large size shells
            {
                innerRingMargin = 5;
                middleRingMargin = 10;
                outerRingMargin = 30;
            }

            if (structureSet.Structures.Any(x => x.Id == ringInnerId))
                ringInner = structureSet.Structures.Where(x => x.Id == ringInnerId).FirstOrDefault();
            else
                ringInner = structureSet.AddStructure("CONTROL", ringInnerId);  //doesnt exist yet
            if (structureSet.Structures.Any(x => x.Id == ringMiddleId))
                ringMiddle = structureSet.Structures.Where(x => x.Id == ringMiddleId).FirstOrDefault();
            else
                ringMiddle = structureSet.AddStructure("CONTROL", ringMiddleId);  //doesnt exist yet
            if (structureSet.Structures.Any(x => x.Id == ringOuterId))
                ringOuter = structureSet.Structures.Where(x => x.Id == ringOuterId).FirstOrDefault();
            else
                ringOuter = structureSet.AddStructure("CONTROL", ringOuterId);  //doesnt exist yet

            ringInner.ConvertToHighResolution();
            ringMiddle.ConvertToHighResolution();
            ringOuter.ConvertToHighResolution();

            ringInner.SegmentVolume = ptvTotal.Margin(innerRingMargin);
            ringMiddle.SegmentVolume = ptvTotal.Margin(middleRingMargin);
            ringOuter.SegmentVolume = ptvTotal.Margin(outerRingMargin);

            ringInner.SegmentVolume = ringInner.Sub(ptvTotal);
            ringMiddle.SegmentVolume = ringMiddle.Sub(ptvTotal);
            ringOuter.SegmentVolume = ringOuter.Sub(ptvTotal);

            ringMiddle.SegmentVolume = ringMiddle.Sub(ringInner);
            ringOuter.SegmentVolume = ringOuter.Sub(ringInner);
            ringOuter.SegmentVolume = ringOuter.Sub(ringMiddle);

            ringInner.SegmentVolume = ringInner.And(structureSet.Structures.Single(x => x.Id == "BODY"));  //clear outside of body
            ringMiddle.SegmentVolume = ringMiddle.And(structureSet.Structures.Single(x => x.Id == "BODY"));  //clear outside of body
            ringOuter.SegmentVolume = ringOuter.And(structureSet.Structures.Single(x => x.Id == "BODY"));  //clear outside of body

            structureSet.RemoveStructure(ptvTotal);
        }
    }
}
