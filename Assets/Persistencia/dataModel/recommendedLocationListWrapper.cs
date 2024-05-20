using System.Collections.Generic;

public class RecommendedLocationWrapper
{
    public List<RecommendedLocationList> recommendedLocationLists;

    public RecommendedLocationWrapper(List<RecommendedLocationList> recommendedLocationLists)
    {
        this.recommendedLocationLists = recommendedLocationLists;
    }
}