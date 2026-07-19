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
            public ChessPieceColour PlayerColour => PlayerInfo.GetPlayerColour(PlayerIndex);
            
            public readonly int PlayerIndex;
            public readonly IPlayerInfo PlayerInfo;
            public readonly ChessBoard ChessBoard;
            public readonly ChessBoardCommandExecutor CommandExecutor;
            public readonly ILuaRuntime LuaRuntime;
            public readonly IRNG RNG;

            public Args(
                int playerIndex,
                IPlayerInfo playerInfo,
                ChessBoard chessBoard,
                ChessBoardCommandExecutor commandExecutor,
                ILuaRuntime luaRuntime,
                IRNG rng)
            {
                PlayerIndex = playerIndex;
                PlayerInfo = playerInfo;
                ChessBoard = chessBoard;
                CommandExecutor = commandExecutor;
                LuaRuntime = luaRuntime;
                RNG = rng;
            }
        }

        UniTask OnTurnBegin(Args args);
        UniTask<ICommand<ChessBoardCommandArgs>> CalculateNextAction(Args args);
        UniTask OnTurnEnd(Args args);
    }
}