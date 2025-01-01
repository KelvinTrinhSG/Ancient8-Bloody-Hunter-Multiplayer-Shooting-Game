using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Thirdweb;
using System.Numerics;
using TMPro;
using UnityEngine.SceneManagement;

public class ConnectWalletManager : MonoBehaviour
{
    public void PlayGame() {
        SceneManager.LoadScene("ShopAndPlay");
    }

    public string Address { get; private set; }
    public Button backButton;

    public TMP_Text rank1WalletAddress;
    public TMP_Text rank2WalletAddress;
    public TMP_Text rank3WalletAddress;
    public TMP_Text rank4WalletAddress;
    public TMP_Text rank5WalletAddress;
    public TMP_Text rank1ScoreText;
    public TMP_Text rank2ScoreText;
    public TMP_Text rank3ScoreText;
    public TMP_Text rank4ScoreText;
    public TMP_Text rank5ScoreText;
    public TMP_Text bestScoreText;

    public GameObject loadingPanel;

    private string leaderboardContractAddress = "0xbccE502f46D8a2098c6621318D7DBfA912b76344";
    private string abiString = "[{\"type\":\"constructor\",\"name\":\"\",\"inputs\":[],\"outputs\":[],\"stateMutability\":\"nonpayable\"},{\"type\":\"event\",\"name\":\"NewHighScore\",\"inputs\":[{\"type\":\"address\",\"name\":\"player\",\"indexed\":true,\"internalType\":\"address\"},{\"type\":\"uint256\",\"name\":\"score\",\"indexed\":false,\"internalType\":\"uint256\"}],\"outputs\":[],\"anonymous\":false},{\"type\":\"event\",\"name\":\"PlayerScoreUpdated\",\"inputs\":[{\"type\":\"address\",\"name\":\"player\",\"indexed\":true,\"internalType\":\"address\"},{\"type\":\"uint256\",\"name\":\"score\",\"indexed\":false,\"internalType\":\"uint256\"}],\"outputs\":[],\"anonymous\":false},{\"type\":\"function\",\"name\":\"getHighScore\",\"inputs\":[],\"outputs\":[{\"type\":\"uint256\",\"name\":\"\",\"internalType\":\"uint256\"}],\"stateMutability\":\"view\"},{\"type\":\"function\",\"name\":\"getPlayerHighScore\",\"inputs\":[{\"type\":\"address\",\"name\":\"playerAddress\",\"internalType\":\"address\"}],\"outputs\":[{\"type\":\"uint256\",\"name\":\"\",\"internalType\":\"uint256\"}],\"stateMutability\":\"view\"},{\"type\":\"function\",\"name\":\"getTopPlayer1Address\",\"inputs\":[],\"outputs\":[{\"type\":\"address\",\"name\":\"\",\"internalType\":\"address\"}],\"stateMutability\":\"view\"},{\"type\":\"function\",\"name\":\"getTopPlayer1Score\",\"inputs\":[],\"outputs\":[{\"type\":\"uint256\",\"name\":\"\",\"internalType\":\"uint256\"}],\"stateMutability\":\"view\"},{\"type\":\"function\",\"name\":\"getTopPlayer2Address\",\"inputs\":[],\"outputs\":[{\"type\":\"address\",\"name\":\"\",\"internalType\":\"address\"}],\"stateMutability\":\"view\"},{\"type\":\"function\",\"name\":\"getTopPlayer2Score\",\"inputs\":[],\"outputs\":[{\"type\":\"uint256\",\"name\":\"\",\"internalType\":\"uint256\"}],\"stateMutability\":\"view\"},{\"type\":\"function\",\"name\":\"getTopPlayer3Address\",\"inputs\":[],\"outputs\":[{\"type\":\"address\",\"name\":\"\",\"internalType\":\"address\"}],\"stateMutability\":\"view\"},{\"type\":\"function\",\"name\":\"getTopPlayer3Score\",\"inputs\":[],\"outputs\":[{\"type\":\"uint256\",\"name\":\"\",\"internalType\":\"uint256\"}],\"stateMutability\":\"view\"},{\"type\":\"function\",\"name\":\"getTopPlayer4Address\",\"inputs\":[],\"outputs\":[{\"type\":\"address\",\"name\":\"\",\"internalType\":\"address\"}],\"stateMutability\":\"view\"},{\"type\":\"function\",\"name\":\"getTopPlayer4Score\",\"inputs\":[],\"outputs\":[{\"type\":\"uint256\",\"name\":\"\",\"internalType\":\"uint256\"}],\"stateMutability\":\"view\"},{\"type\":\"function\",\"name\":\"getTopPlayer5Address\",\"inputs\":[],\"outputs\":[{\"type\":\"address\",\"name\":\"\",\"internalType\":\"address\"}],\"stateMutability\":\"view\"},{\"type\":\"function\",\"name\":\"getTopPlayer5Score\",\"inputs\":[],\"outputs\":[{\"type\":\"uint256\",\"name\":\"\",\"internalType\":\"uint256\"}],\"stateMutability\":\"view\"},{\"type\":\"function\",\"name\":\"getTopPlayers\",\"inputs\":[],\"outputs\":[{\"type\":\"tuple[5]\",\"name\":\"\",\"components\":[{\"type\":\"address\",\"name\":\"playerAddress\",\"internalType\":\"address\"},{\"type\":\"uint256\",\"name\":\"score\",\"internalType\":\"uint256\"}],\"internalType\":\"struct HighScore.Player[5]\"}],\"stateMutability\":\"view\"},{\"type\":\"function\",\"name\":\"highestScore\",\"inputs\":[],\"outputs\":[{\"type\":\"uint256\",\"name\":\"\",\"internalType\":\"uint256\"}],\"stateMutability\":\"view\"},{\"type\":\"function\",\"name\":\"owner\",\"inputs\":[],\"outputs\":[{\"type\":\"address\",\"name\":\"\",\"internalType\":\"address\"}],\"stateMutability\":\"view\"},{\"type\":\"function\",\"name\":\"playerHighScores\",\"inputs\":[{\"type\":\"address\",\"name\":\"\",\"internalType\":\"address\"}],\"outputs\":[{\"type\":\"uint256\",\"name\":\"\",\"internalType\":\"uint256\"}],\"stateMutability\":\"view\"},{\"type\":\"function\",\"name\":\"setPlayerScore\",\"inputs\":[{\"type\":\"address\",\"name\":\"playerAddress\",\"internalType\":\"address\"},{\"type\":\"uint256\",\"name\":\"newScore\",\"internalType\":\"uint256\"}],\"outputs\":[],\"stateMutability\":\"nonpayable\"},{\"type\":\"function\",\"name\":\"topPlayers\",\"inputs\":[{\"type\":\"uint256\",\"name\":\"\",\"internalType\":\"uint256\"}],\"outputs\":[{\"type\":\"address\",\"name\":\"playerAddress\",\"internalType\":\"address\"},{\"type\":\"uint256\",\"name\":\"score\",\"internalType\":\"uint256\"}],\"stateMutability\":\"view\"}]";
    private void Start()
    {
        loadingPanel.SetActive(true);
        DisplayLeaderboard();
    }

    public async void GetHighestScore()
    {
        Address = await ThirdwebManager.Instance.SDK.Wallet.GetAddress();
        var contract = ThirdwebManager.Instance.SDK.GetContract(
            leaderboardContractAddress,
            abiString
            );
        BigInteger highestScoreOfPlayer = await contract.Read<BigInteger>("getPlayerHighScore", Address);
        Debug.Log(highestScoreOfPlayer);
        bestScoreText.text = FormatNumberWithCommas(highestScoreOfPlayer.ToString());

        loadingPanel.SetActive(false);
    }

    string FormatNumberWithCommas(string numberString)
    {
        if (long.TryParse(numberString, out long number))
        {
            return number.ToString("N0");
        }
        else
        {
            return numberString;
        }
    }

    string ShortenString(string input)
    {
        if (input.Length <= 10)
        {
            return input;
        }
        return input.Substring(0, 6) + "…" + input.Substring(input.Length - 4);
    }

    public async void DisplayLeaderboard()
    {
        var contract = ThirdwebManager.Instance.SDK.GetContract(
           leaderboardContractAddress,
           abiString
           );
        rank1WalletAddress.text = ShortenString(await contract.Read<string>("getTopPlayer1Address"));
        rank2WalletAddress.text = ShortenString(await contract.Read<string>("getTopPlayer2Address"));
        rank3WalletAddress.text = ShortenString(await contract.Read<string>("getTopPlayer3Address"));
        rank4WalletAddress.text = ShortenString(await contract.Read<string>("getTopPlayer4Address"));
        rank5WalletAddress.text = ShortenString(await contract.Read<string>("getTopPlayer5Address"));
        rank1ScoreText.text = FormatNumberWithCommas(await contract.Read<string>("getTopPlayer1Score"));
        rank2ScoreText.text = FormatNumberWithCommas(await contract.Read<string>("getTopPlayer2Score"));
        rank3ScoreText.text = FormatNumberWithCommas(await contract.Read<string>("getTopPlayer3Score"));
        rank4ScoreText.text = FormatNumberWithCommas(await contract.Read<string>("getTopPlayer4Score"));
        rank5ScoreText.text = FormatNumberWithCommas(await contract.Read<string>("getTopPlayer5Score"));
    }

}
