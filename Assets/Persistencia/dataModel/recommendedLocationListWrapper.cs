using System.Collections.Generic;

public class RecommendedLocationWrapper
{
    public List<RecommendedLocationList> recommendedLists;
    public RecommendedLocationWrapper(List<RecommendedLocationList> recommendedLists)
    {
        this.recommendedLists = recommendedLists;
    }
}