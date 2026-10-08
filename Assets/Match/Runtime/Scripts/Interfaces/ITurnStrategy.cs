using Celeste.Core.Interfaces;
using Celeste.Lua;
using Cysharp.Threading.Tasks;
using QC.Match.Commands;
using QC.Match.Objects;
using QC.Match.Players;

namespace QC.Match
{
    public interface ITurnStrategy
    {
        public readonly struct Args
        {
            public PieceColour PlayerColour => PlayerInfo.GetPlayerColour(PlayerIndex);
            
            public readonly int PlayerIndex;
            public readonly IPlayerInfo PlayerInfo;
            public readonly IBoardActionMaskLookup BoardActionMaskLookup;
            public readonly Board Board;
            public readonly BoardCommandExecutor CommandExecutor;
            public readonly ILuaRuntime LuaRuntime;
            public readonly IRNG RNG;

            public Args(
                int playerIndex,
                IPlayerInfo playerInfo,
                IBoardActionMaskLookup boardActionMaskLookup,
                Board board,
                BoardCommandExecutor commandExecutor,
                ILuaRuntime luaRuntime,
                IRNG rng)
            {
                PlayerIndex = playerIndex;
                PlayerInfo = playerInfo;
                BoardActionMaskLookup = boardActionMaskLookup;
                Board = board;
                CommandExecutor = commandExecutor;
                LuaRuntime = luaRuntime;
                RNG = rng;
            }
        }

        UniTask OnTurnBegin(Args args);
        UniTask OnTurn(Args args, bool dryRun);
        UniTask OnTurnEnd(Args args);
    }
}