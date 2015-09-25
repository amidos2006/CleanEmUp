using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CleanEmUp
{
    public enum SteamErrors
    {
        NoError,
        FailInit,
        AchievementFailed,
        WrongAssembly
    }

    public class SteamManager
    {
        private static SteamLeaderboard_t[] leaderBoards;

        private static void ScoreLeaderBoardCallBack(LeaderboardFindResult_t result, bool failure)
        {
            leaderBoards[0] = result.m_hSteamLeaderboard;
            Console.WriteLine(result.m_bLeaderboardFound);
        }

        private static void TimeLeaderBoardCallBack(LeaderboardFindResult_t result, bool failure)
        {
            leaderBoards[1] = result.m_hSteamLeaderboard;
        }

        private static void FileLeaderBoardCallBack(LeaderboardFindResult_t result, bool failure)
        {
            leaderBoards[2] = result.m_hSteamLeaderboard;
        }

        public static SteamErrors Initialize() 
        { 
            if(!SteamAPI.Init())
            {
                return SteamErrors.FailInit;
            }

            if (!Packsize.Test())
            {
                return SteamErrors.WrongAssembly;
            }

            CallResult<LeaderboardFindResult_t> scoreCallBack = CallResult<LeaderboardFindResult_t>.Create(ScoreLeaderBoardCallBack);
            SteamAPICall_t functionCall = SteamUserStats.FindLeaderboard("Score Survival");
            scoreCallBack.Set(functionCall);

            return SteamErrors.NoError;
        }

        public static SteamErrors SubmitScore(int index, int score)
        {
            SteamUserStats.UploadLeaderboardScore(leaderBoards[index], 
                ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest, score, null, 0);
            return SteamErrors.NoError;
        }

        public static SteamErrors SetAcheivement(string name)
        {
            if (!SteamUserStats.SetAchievement(name)) 
            {
                return SteamErrors.AchievementFailed;
            }
            return SteamErrors.NoError;
        }

        public static void CloseSteam()
        {
            SteamAPI.Shutdown();
        }

        public static void Update()
        {
            //SteamAPI.RunCallbacks();
        }
    }
}
