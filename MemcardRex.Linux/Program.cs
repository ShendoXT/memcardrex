/* Program.cs - MemcardRex for Linux
 *
 * Copyright (C) 2024 Rob Hall
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 *
 * SPDX-License-Identifier: GPL-3.0-or-later
 */

namespace MemcardRex.Linux;

public partial class Program(string[] args)
{
    private readonly Application app = Application.New("io.github.robxnano.MemcardRex", Gio.ApplicationFlags.FlagsNone);

    public static int Main(string[] args) => new Program(args).Run();

    public int Run()
    {
        Utils.CheckLibraryVersions();
        try
        {
            return app.RunWithSynchronizationContext(args);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine($"\n\n{ex.StackTrace}");
            return -1;
        }
    }
}