using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDSTools {
    //Utility
    public class constantsContainer {
        public const int layer_floor = -10;
        public const int layer_walls = -1;
        public const int layer_detail = 0;
        public const int layer_props = 1;
        public const int layer_projectiles = 2;
        public const int layer_actors = 3;
        public const int layer_player = 4;
        public const int layer_particles = 5;
        public const int layer_ceiling = 10;
    }

    public enum keys {
        red,
        green,
        blue,
        yellow,
        pink,
        white,
        none
    }

    public enum ammoType
    {
        pistol,
        shotgun,
        smg,
        dynamite
    }
    
    public struct aiTarget 
    {
        public Vector3 pos;
        public bool isNotChecked;
        public bool isVisual;
        public float interest;

        public static bool operator ==(aiTarget a, aiTarget b) {
            return areEqual(a, b);
        }

        public static bool operator !=(aiTarget a, aiTarget b) {
            return !areEqual(a, b);
        }

        private static bool areEqual(aiTarget a, aiTarget b) {
            return (
                (a.pos == b.pos) &&
                (a.isNotChecked == b.isNotChecked) && 
                (a.isVisual == b.isVisual)
            ); //&&(a.interest == b.interest)
        }

        public aiTarget(Vector3 pos) 
        {
            this.pos = pos;
            this.isNotChecked = false;
            this.isVisual = false;
            this.interest = 0;
        }

        public aiTarget(Vector3 pos, bool isNotChecked) 
        {
            this.pos = pos;
            this.isNotChecked = isNotChecked;
            this.isVisual = false;
            this.interest = 0;
        }

        public aiTarget(Vector3 pos, bool isNotChecked, bool isVisual) 
        {
            this.pos = pos;
            this.isNotChecked = isNotChecked;
            this.isVisual = isVisual;
            this.interest = isVisual ? 10 : 0;
        }

        public aiTarget(Vector3 pos, bool isNotChecked, bool isVisual, float interest) 
        {
            this.pos = pos;
            this.isNotChecked = isNotChecked;
            this.isVisual = isVisual;
            this.interest = isVisual ? 10 : interest;
        }
    };

    public struct PlayerPack 
    {
        //Packs a player actor and player inventory together
        public playerActor plrAct;
        public playerInventory plrInv;

        public PlayerPack(playerActor plrAct, playerInventory plrInv) 
        {
            this.plrAct = plrAct;
            this.plrInv = plrInv;
        }
    };
}