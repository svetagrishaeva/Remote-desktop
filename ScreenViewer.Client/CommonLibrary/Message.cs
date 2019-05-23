using System;
using System.Drawing;

namespace CommonLibrary
{
    [Serializable]
    public struct Message
    {
        public int messageType; //тип сообщения
        public byte[] bytes;
        public Point cursor;
        public Size screenSize;
        public int index;
        public string key;
        public string[] info; //информация, либо список дирректорий
        public string[] files;
        public string[,] proc;
    }
}
