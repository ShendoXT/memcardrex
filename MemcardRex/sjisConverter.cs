//Shift-JIS converter class
//Shendo 2009-2021

using System;
using System.Collections.Generic;
using System.Text;

namespace MemcardRex
{
    class charConverter
    {
        //Convert SJIS characters to ASCII equivalent
        public string convertSJIStoASCII(byte[] bData)
        {
            string output = null;

            //Start from title offset
            for (int bCounter = 4; bCounter < 0x44; bCounter+=2)
            {
                switch ((bData[bCounter]<<8) | bData[bCounter+1])
                {
                    default:        //Character not found
                        break;

                    case 0x0000:    //End of the string
                        return output;

                    case 0x8140:
                        output += "  ";
                        break;

                    case 0x8143:
                        output += ',';
                        break;

                    case 0x8144:
                        output += '.';
                        break;

                    case 0x8145:
                        output += '·';
                        break;

                    case 0x8146:
                        output += ':';
                        break;

                    case 0x8147:
                        output += ';';
                        break;

                    case 0x8148:
                        output += '?';
                        break;

                    case 0x8149:
                        output += '!';
                        break;

                    case 0x814F:
                        output += '^';
                        break;

                    case 0x8151:
                        output += '_';
                        break;

                    case 0x815B:
                    case 0x815C:
                    case 0x815D:
                        output += '-';
                        break;

                    case 0x815E:
                        output += '/';
                        break;

                    case 0x815F:
                        output += '\\';
                        break;

                    case 0x8160:
                        output += '~';
                        break;

                    case 0x8161:
                        output += '|';
                        break;

                    case 0x8168:
                        output += "\"";
                        break;

                    case 0x8169:
                        output += '(';
                        break;

                    case 0x816A:
                        output += ')';
                        break;

                    case 0x816D:
                        output += '[';
                        break;

                    case 0x816E:
                        output += ']';
                        break;

                    case 0x816F:
                        output += '{';
                        break;

                    case 0x8170:
                        output += '}';
                        break;

                    case 0x817B:
                        output += '+';
                        break;

                    case 0x817C:
                        output += '-';
                        break;

                    case 0x817D:
                        output += '±';
                        break;

                    case 0x817E:
                        output += '*';
                        break;

                    case 0x8180:
                        output += '÷';
                            break;

                    case 0x8181:
                        output += '=';
                        break;

                    case 0x8183:
                        output += '<';
                        break;

                    case 0x8184:
                        output += '>';
                        break;

                    case 0x818A:
                        output += '°';
                        break;

                    case 0x818B:
                        output += '\'';
                        break;

                    case 0x818C:
                        output += '\"';
                        break;

                    case 0x8190:
                        output += '$';
                        break;

                    case 0x8193:
                        output += '%';
                        break;

                    case 0x8194:
                        output += '#';
                        break;

                    case 0x8195:
                        output += '&';
                        break;

                    case 0x8196:
                        output += '*';
                        break;
                        
                    case 0x8197:
                        output += '@';
                        break;

                    case 0x824F:
                        output += '0';
                        break;

                    case 0x8250:
                        output += '1';
                        break;

                    case 0x8251:
                        output += '2';
                        break;

                    case 0x8252:
                        output += '3';
                        break;

                    case 0x8253:
                        output += '4';
                        break;

                    case 0x8254:
                        output += '5';
                        break;

                    case 0x8255:
                        output += '6';
                        break;

                    case 0x8256:
                        output += '7';
                        break;

                    case 0x8257:
                        output += '8';
                        break;

                    case 0x8258:
                        output += '9';
                        break;

                    case 0x8260:
                        output += 'A';
                        break;

                    case 0x8261:
                        output += 'B';
                        break;

                    case 0x8262:
                        output += 'C';
                        break;

                    case 0x8263:
                        output += 'D';
                        break;

                    case 0x8264:
                        output += 'E';
                        break;

                    case 0x8265:
                        output += 'F';
                        break;

                    case 0x8266:
                        output += 'G';
                        break;

                    case 0x8267:
                        output += 'H';
                        break;

                    case 0x8268:
                        output += 'I';
                        break;

                    case 0x8269:
                        output += 'J';
                        break;

                    case 0x826A:
                        output += 'K';
                        break;

                    case 0x826B:
                        output += 'L';
                        break;

                    case 0x826C:
                        output += 'M';
                        break;

                    case 0x826D:
                        output += 'N';
                        break;

                    case 0x826E:
                        output += 'O';
                        break;

                    case 0x826F:
                        output += 'P';
                        break;

                    case 0x8270:
                        output += 'Q';
                        break;

                    case 0x8271:
                        output += 'R';
                        break;

                    case 0x8272:
                        output += 'S';
                        break;

                    case 0x8273:
                        output += 'T';
                        break;

                    case 0x8274:
                        output += 'U';
                        break;

                    case 0x8275:
                        output += 'V';
                        break;

                    case 0x8276:
                        output += 'W';
                        break;

                    case 0x8277:
                        output += 'X';
                        break;

                    case 0x8278:
                        output += 'Y';
                        break;

                    case 0x8279:
                        output += 'Z';
                        break;

                    case 0x8281:
                        output += 'a';
                        break;

                    case 0x8282:
                        output += 'b';
                        break;

                    case 0x8283:
                        output += 'c';
                        break;

                    case 0x8284:
                        output += 'd';
                        break;

                    case 0x8285:
                        output += 'e';
                        break;

                    case 0x8286:
                        output += 'f';
                        break;

                    case 0x8287:
                        output += 'g';
                        break;

                    case 0x8288:
                        output += 'h';
                        break;

                    case 0x8289:
                        output += 'i';
                        break;

                    case 0x828A:
                        output += 'j';
                        break;

                    case 0x828B:
                        output += 'k';
                        break;

                    case 0x828C:
                        output += 'l';
                        break;

                    case 0x828D:
                        output += 'm';
                        break;

                    case 0x828E:
                        output += 'n';
                        break;

                    case 0x828F:
                        output += 'o';
                        break;

                    case 0x8290:
                        output += 'p';
                        break;

                    case 0x8291:
                        output += 'q';
                        break;

                    case 0x8292:
                        output += 'r';
                        break;

                    case 0x8293:
                        output += 's';
                        break;

                    case 0x8294:
                        output += 't';
                        break;

                    case 0x8295:
                        output += 'u';
                        break;

                    case 0x8296:
                        output += 'v';
                        break;

                    case 0x8297:
                        output += 'w';
                        break;

                    case 0x8298:
                        output += 'x';
                        break;

                    case 0x8299:
                        output += 'y';
                        break;

                    case 0x829A:
                        output += 'z';
                        break;
                }

            }

            return output;
        }
    }
}
