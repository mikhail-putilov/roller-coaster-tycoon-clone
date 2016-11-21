using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;


public static class Grammar {

    public static string getNext(string current, bool isTurn, bool isRight, bool turnNear, int height, int remains) {
        //List<String> possible = new List<string>();       
        //----------4
        if (turnNear && !isTurn) {
            if (String.Compare(current, "u") == 0 || String.Compare(current, "tu") == 0) {
                return "tsu";
            }

            if (String.Compare(current, "d") == 0 || String.Compare(current, "td") == 0) {
                return "tsd";
            }

            if (String.Compare(current, "s") == 0 || String.Compare(current, "tsu") == 0 || String.Compare(current, "tsd") == 0) {
                return "s";
            }
        }
        //----------1
        if (height >= 1 && !isTurn && !turnNear && height < remains) {
            if (String.Compare(current, "s") == 0 || String.Compare(current, "l") == 0 || String.Compare(current, "r") == 0 || String.Compare(current, "tsd") == 0
                || String.Compare(current, "tsu") == 0) {
                List<String> possible = new List<string>();
                possible.Add("s");
                possible.Add("tu");
                possible.Add("td");
                return possible[UnityEngine.Random.Range(0, possible.Count)];
            }

            if (String.Compare(current, "tu") == 0) {
                List<String> possible = new List<string>();
                possible.Add("u");
                possible.Add("tsu");
                return possible[UnityEngine.Random.Range(0, possible.Count)];
            }

            if (String.Compare(current, "td") == 0) {
                List<String> possible = new List<string>();
                possible.Add("d");
                possible.Add("tsd");
                return possible[UnityEngine.Random.Range(0, possible.Count)];
            }

            if (String.Compare(current, "d") == 0) {
                List<String> possible = new List<string>();
                possible.Add("d");
                possible.Add("tsd");
                return possible[UnityEngine.Random.Range(0, possible.Count)];
            }
            if (String.Compare(current, "u") == 0) {
                List<String> possible = new List<string>();
                possible.Add("u");
                possible.Add("tsu");
                return possible[UnityEngine.Random.Range(0, possible.Count)];
            }
        }

        //----------2
        if (height >= remains && !isTurn && !turnNear && height > 1) {
            if (String.Compare(current, "u") == 0 || String.Compare(current, "tu") == 0) {
                return "tsu";
            }
            if (String.Compare(current, "tsd") == 0 || String.Compare(current, "s") == 0 || String.Compare(current, "tsu") == 0
                || String.Compare(current, "l") == 0 || String.Compare(current, "r") == 0) {
                return "td";
            }
            if (String.Compare(current, "td") == 0 || String.Compare(current, "d") == 0) {
                return "d";
            }

        }

        //----------3
        if (height < 1 && !isTurn && !turnNear) {
            if (String.Compare(current, "s") == 0 || String.Compare(current, "l") == 0 || String.Compare(current, "r") == 0
                || String.Compare(current, "tsd") == 0 || String.Compare(current, "tsu") == 0) {
                List<String> possible = new List<string>();
                possible.Add("tu");
                possible.Add("s");
                return possible[UnityEngine.Random.Range(0, possible.Count)];
            }

            if (String.Compare(current, "d") == 0|| String.Compare(current, "td") == 0) {
                return "tsd";
            }

            if (String.Compare(current, "tu") == 0 || String.Compare(current, "u") == 0) {
                List<String> possible = new List<string>();
                possible.Add("tsu");
                possible.Add("u");
                return possible[UnityEngine.Random.Range(0, possible.Count)];
            }
        }
     


        //----------turns
        if (isTurn && isRight) {
            return "r";
        }

        if (isTurn && !isRight) {
            return "l";
        }

        return "s";
    }



}

