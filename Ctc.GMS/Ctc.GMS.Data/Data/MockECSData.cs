namespace GMS.Data;

/// <summary>
/// Mock ECS (Educator Credentialing System) data for GMS integration
/// Simulates data that would come from ECS APIs in production
/// </summary>
public static class MockECSData
{
    #region Static Data Properties

    public static List<ECSCounty> Counties { get; } = new()
    {
        new() { Id = 1, Code = "01", Name = "Alameda" },
        new() { Id = 2, Code = "02", Name = "Alpine" },
        new() { Id = 3, Code = "03", Name = "Amador" },
        new() { Id = 4, Code = "04", Name = "Butte" },
        new() { Id = 5, Code = "05", Name = "Calaveras" },
        new() { Id = 6, Code = "06", Name = "Colusa" },
        new() { Id = 7, Code = "07", Name = "Contra Costa" },
        new() { Id = 8, Code = "08", Name = "Del Norte" },
        new() { Id = 9, Code = "09", Name = "El Dorado" },
        new() { Id = 10, Code = "10", Name = "Fresno" },
        new() { Id = 11, Code = "11", Name = "Glenn" },
        new() { Id = 12, Code = "12", Name = "Humboldt" },
        new() { Id = 13, Code = "13", Name = "Imperial" },
        new() { Id = 14, Code = "14", Name = "Inyo" },
        new() { Id = 15, Code = "15", Name = "Kern" },
        new() { Id = 16, Code = "16", Name = "Kings" },
        new() { Id = 17, Code = "17", Name = "Lake" },
        new() { Id = 18, Code = "18", Name = "Lassen" },
        new() { Id = 19, Code = "19", Name = "Los Angeles" },
        new() { Id = 20, Code = "20", Name = "Madera" },
        new() { Id = 21, Code = "21", Name = "Marin" },
        new() { Id = 22, Code = "22", Name = "Mariposa" },
        new() { Id = 23, Code = "23", Name = "Mendocino" },
        new() { Id = 24, Code = "24", Name = "Merced" },
        new() { Id = 25, Code = "25", Name = "Modoc" },
        new() { Id = 26, Code = "26", Name = "Mono" },
        new() { Id = 27, Code = "27", Name = "Monterey" },
        new() { Id = 28, Code = "28", Name = "Napa" },
        new() { Id = 29, Code = "29", Name = "Nevada" },
        new() { Id = 30, Code = "30", Name = "Orange" },
        new() { Id = 31, Code = "31", Name = "Placer" },
        new() { Id = 32, Code = "32", Name = "Plumas" },
        new() { Id = 33, Code = "33", Name = "Riverside" },
        new() { Id = 34, Code = "34", Name = "Sacramento" },
        new() { Id = 35, Code = "35", Name = "San Benito" },
        new() { Id = 36, Code = "36", Name = "San Bernardino" },
        new() { Id = 37, Code = "37", Name = "San Diego" },
        new() { Id = 38, Code = "38", Name = "San Francisco" },
        new() { Id = 39, Code = "39", Name = "San Joaquin" },
        new() { Id = 40, Code = "40", Name = "San Luis Obispo" },
        new() { Id = 41, Code = "41", Name = "San Mateo" },
        new() { Id = 42, Code = "42", Name = "Santa Barbara" },
        new() { Id = 43, Code = "43", Name = "Santa Clara" },
        new() { Id = 44, Code = "44", Name = "Santa Cruz" },
        new() { Id = 45, Code = "45", Name = "Shasta" },
        new() { Id = 46, Code = "46", Name = "Sierra" },
        new() { Id = 47, Code = "47", Name = "Siskiyou" },
        new() { Id = 48, Code = "48", Name = "Solano" },
        new() { Id = 49, Code = "49", Name = "Sonoma" },
        new() { Id = 50, Code = "50", Name = "Stanislaus" },
        new() { Id = 51, Code = "51", Name = "Sutter" },
        new() { Id = 52, Code = "52", Name = "Tehama" },
        new() { Id = 53, Code = "53", Name = "Trinity" },
        new() { Id = 54, Code = "54", Name = "Tulare" },
        new() { Id = 55, Code = "55", Name = "Tuolumne" },
        new() { Id = 56, Code = "56", Name = "Ventura" },
        new() { Id = 57, Code = "57", Name = "Yolo" },
        new() { Id = 58, Code = "58", Name = "Yuba" }
    };

    public static List<ECSDistrict> Districts { get; } = new()
    {
        // Los Angeles County (19)
        new() { Id = 1, CountyCode = "19", CdsCode = "1964733", Name = "Los Angeles Unified", CountyName = "Los Angeles" },
        new() { Id = 2, CountyCode = "19", CdsCode = "1964949", Name = "Long Beach Unified", CountyName = "Los Angeles" },
        new() { Id = 3, CountyCode = "19", CdsCode = "1964782", Name = "Pasadena Unified", CountyName = "Los Angeles" },
        new() { Id = 4, CountyCode = "19", CdsCode = "1964857", Name = "Glendale Unified", CountyName = "Los Angeles" },
        new() { Id = 5, CountyCode = "19", CdsCode = "1965060", Name = "Santa Monica-Malibu Unified", CountyName = "Los Angeles" },
        new() { Id = 6, CountyCode = "19", CdsCode = "1964592", Name = "Compton Unified", CountyName = "Los Angeles" },
        new() { Id = 7, CountyCode = "19", CdsCode = "1964691", Name = "Inglewood Unified", CountyName = "Los Angeles" },
        new() { Id = 8, CountyCode = "19", CdsCode = "1975440", Name = "Torrance Unified", CountyName = "Los Angeles" },

        // San Diego County (37)
        new() { Id = 9, CountyCode = "37", CdsCode = "3768338", Name = "San Diego Unified", CountyName = "San Diego" },
        new() { Id = 10, CountyCode = "37", CdsCode = "3768171", Name = "Sweetwater Union High", CountyName = "San Diego" },
        new() { Id = 11, CountyCode = "37", CdsCode = "3768049", Name = "Poway Unified", CountyName = "San Diego" },
        new() { Id = 12, CountyCode = "37", CdsCode = "3768080", Name = "Chula Vista Elementary", CountyName = "San Diego" },

        // Orange County (30)
        new() { Id = 13, CountyCode = "30", CdsCode = "3066621", Name = "Santa Ana Unified", CountyName = "Orange" },
        new() { Id = 14, CountyCode = "30", CdsCode = "3066399", Name = "Anaheim Union High", CountyName = "Orange" },
        new() { Id = 15, CountyCode = "30", CdsCode = "3066464", Name = "Garden Grove Unified", CountyName = "Orange" },
        new() { Id = 16, CountyCode = "30", CdsCode = "3066670", Name = "Irvine Unified", CountyName = "Orange" },

        // Fresno County (10)
        new() { Id = 17, CountyCode = "10", CdsCode = "1062166", Name = "Fresno Unified", CountyName = "Fresno" },
        new() { Id = 18, CountyCode = "10", CdsCode = "1062000", Name = "Central Unified", CountyName = "Fresno" },
        new() { Id = 19, CountyCode = "10", CdsCode = "1062281", Name = "Clovis Unified", CountyName = "Fresno" },

        // Sacramento County (34)
        new() { Id = 20, CountyCode = "34", CdsCode = "3467447", Name = "Sacramento City Unified", CountyName = "Sacramento" },
        new() { Id = 21, CountyCode = "34", CdsCode = "3467439", Name = "Elk Grove Unified", CountyName = "Sacramento" },
        new() { Id = 22, CountyCode = "34", CdsCode = "3467272", Name = "San Juan Unified", CountyName = "Sacramento" },
        new() { Id = 23, CountyCode = "34", CdsCode = "3467314", Name = "Natomas Unified", CountyName = "Sacramento" },
        new() { Id = 24, CountyCode = "34", CdsCode = "3467355", Name = "Twin Rivers Unified", CountyName = "Sacramento" },

        // San Francisco County (38)
        new() { Id = 25, CountyCode = "38", CdsCode = "3868478", Name = "San Francisco Unified", CountyName = "San Francisco" },

        // Alameda County (01)
        new() { Id = 26, CountyCode = "01", CdsCode = "0161259", Name = "Oakland Unified", CountyName = "Alameda" },
        new() { Id = 27, CountyCode = "01", CdsCode = "0161143", Name = "Fremont Unified", CountyName = "Alameda" },
        new() { Id = 28, CountyCode = "01", CdsCode = "0161176", Name = "Hayward Unified", CountyName = "Alameda" },

        // Santa Clara County (43)
        new() { Id = 29, CountyCode = "43", CdsCode = "4369682", Name = "San Jose Unified", CountyName = "Santa Clara" },
        new() { Id = 30, CountyCode = "43", CdsCode = "4369427", Name = "East Side Union High", CountyName = "Santa Clara" },

        // Riverside County (33)
        new() { Id = 31, CountyCode = "33", CdsCode = "3367124", Name = "Riverside Unified", CountyName = "Riverside" },
        new() { Id = 32, CountyCode = "33", CdsCode = "3375093", Name = "Corona-Norco Unified", CountyName = "Riverside" },

        // San Bernardino County (36)
        new() { Id = 33, CountyCode = "36", CdsCode = "3667843", Name = "San Bernardino City Unified", CountyName = "San Bernardino" },
        new() { Id = 34, CountyCode = "36", CdsCode = "3667652", Name = "Fontana Unified", CountyName = "San Bernardino" },

        // Kern County (15)
        new() { Id = 35, CountyCode = "15", CdsCode = "1563321", Name = "Bakersfield City Elementary", CountyName = "Kern" },
        new() { Id = 36, CountyCode = "15", CdsCode = "1563339", Name = "Kern High", CountyName = "Kern" }
    };

    public static List<ECSSchool> Schools { get; } = new()
    {
        // LAUSD Schools (1964733)
        new() { Id = 1, CdsCode = "19647336001234", Name = "Lincoln Elementary", DistrictName = "Los Angeles Unified", Address = "123 Main St", City = "Los Angeles", GradeSpan = "K-5" },
        new() { Id = 2, CdsCode = "19647336001235", Name = "Washington Elementary", DistrictName = "Los Angeles Unified", Address = "456 Oak Ave", City = "Los Angeles", GradeSpan = "K-5" },
        new() { Id = 3, CdsCode = "19647336001236", Name = "Jefferson Middle", DistrictName = "Los Angeles Unified", Address = "789 Pine Blvd", City = "Los Angeles", GradeSpan = "6-8" },
        new() { Id = 4, CdsCode = "19647336001237", Name = "Roosevelt High", DistrictName = "Los Angeles Unified", Address = "321 Elm St", City = "Los Angeles", GradeSpan = "9-12" },
        new() { Id = 5, CdsCode = "19647336001238", Name = "Garfield High", DistrictName = "Los Angeles Unified", Address = "654 Cedar Ave", City = "Los Angeles", GradeSpan = "9-12" },
        new() { Id = 6, CdsCode = "19647336001239", Name = "Belmont High", DistrictName = "Los Angeles Unified", Address = "987 Birch Ln", City = "Los Angeles", GradeSpan = "9-12" },

        // Long Beach Unified Schools (1964949)
        new() { Id = 7, CdsCode = "19649496002001", Name = "Poly High", DistrictName = "Long Beach Unified", Address = "111 Beach Blvd", City = "Long Beach", GradeSpan = "9-12" },
        new() { Id = 8, CdsCode = "19649496002002", Name = "Wilson High", DistrictName = "Long Beach Unified", Address = "222 Ocean Ave", City = "Long Beach", GradeSpan = "9-12" },
        new() { Id = 9, CdsCode = "19649496002003", Name = "Lakewood High", DistrictName = "Long Beach Unified", Address = "333 Harbor Dr", City = "Lakewood", GradeSpan = "9-12" },

        // San Diego Unified Schools (3768338)
        new() { Id = 10, CdsCode = "37683386003001", Name = "San Diego High", DistrictName = "San Diego Unified", Address = "444 Pacific Hwy", City = "San Diego", GradeSpan = "9-12" },
        new() { Id = 11, CdsCode = "37683386003002", Name = "Lincoln High", DistrictName = "San Diego Unified", Address = "555 Market St", City = "San Diego", GradeSpan = "9-12" },
        new() { Id = 12, CdsCode = "37683386003003", Name = "Hoover High", DistrictName = "San Diego Unified", Address = "666 Park Blvd", City = "San Diego", GradeSpan = "9-12" },
        new() { Id = 13, CdsCode = "37683386003004", Name = "Mission Bay High", DistrictName = "San Diego Unified", Address = "777 Mission Bay Dr", City = "San Diego", GradeSpan = "9-12" },

        // Fresno Unified Schools (1062166)
        new() { Id = 14, CdsCode = "10621666004001", Name = "Fresno High", DistrictName = "Fresno Unified", Address = "888 Blackstone Ave", City = "Fresno", GradeSpan = "9-12" },
        new() { Id = 15, CdsCode = "10621666004002", Name = "Edison High", DistrictName = "Fresno Unified", Address = "999 California Ave", City = "Fresno", GradeSpan = "9-12" },
        new() { Id = 16, CdsCode = "10621666004003", Name = "Roosevelt High", DistrictName = "Fresno Unified", Address = "100 Tulare St", City = "Fresno", GradeSpan = "9-12" },

        // Sacramento City Unified Schools (3467447)
        new() { Id = 17, CdsCode = "34674476005001", Name = "C.K. McClatchy High", DistrictName = "Sacramento City Unified", Address = "3066 Freeport Blvd", City = "Sacramento", GradeSpan = "9-12" },
        new() { Id = 18, CdsCode = "34674476005002", Name = "Kennedy High", DistrictName = "Sacramento City Unified", Address = "6715 Gloria Dr", City = "Sacramento", GradeSpan = "9-12" },

        // Natomas Unified Schools (3467314)
        new() { Id = 19, CdsCode = "34673146005101", Name = "Natomas High", DistrictName = "Natomas Unified", Address = "3301 Fong Ranch Rd", City = "Sacramento", GradeSpan = "9-12" },
        new() { Id = 20, CdsCode = "34673146005102", Name = "Inderkum High", DistrictName = "Natomas Unified", Address = "2500 New Market Dr", City = "Sacramento", GradeSpan = "9-12" },
        new() { Id = 21, CdsCode = "34673146005103", Name = "Paso Verde Elementary", DistrictName = "Natomas Unified", Address = "3500 Paso Verde Dr", City = "Sacramento", GradeSpan = "K-5" },

        // Santa Ana Unified Schools (3066621)
        new() { Id = 22, CdsCode = "30666216006001", Name = "Santa Ana High", DistrictName = "Santa Ana Unified", Address = "520 W Walnut St", City = "Santa Ana", GradeSpan = "9-12" },
        new() { Id = 23, CdsCode = "30666216006002", Name = "Century High", DistrictName = "Santa Ana Unified", Address = "1401 S Grand Ave", City = "Santa Ana", GradeSpan = "9-12" },

        // Oakland Unified Schools (0161259)
        new() { Id = 24, CdsCode = "01612596007001", Name = "Oakland High", DistrictName = "Oakland Unified", Address = "1023 MacArthur Blvd", City = "Oakland", GradeSpan = "9-12" },
        new() { Id = 25, CdsCode = "01612596007002", Name = "Skyline High", DistrictName = "Oakland Unified", Address = "12250 Skyline Blvd", City = "Oakland", GradeSpan = "9-12" },

        // San Francisco Unified Schools (3868478)
        new() { Id = 26, CdsCode = "38684786008001", Name = "Lowell High", DistrictName = "San Francisco Unified", Address = "1101 Eucalyptus Dr", City = "San Francisco", GradeSpan = "9-12" },
        new() { Id = 27, CdsCode = "38684786008002", Name = "Balboa High", DistrictName = "San Francisco Unified", Address = "1000 Cayuga Ave", City = "San Francisco", GradeSpan = "9-12" },
        new() { Id = 28, CdsCode = "38684786008003", Name = "Mission High", DistrictName = "San Francisco Unified", Address = "3750 18th St", City = "San Francisco", GradeSpan = "9-12" },

        // Pasadena Unified Schools (1964782)
        new() { Id = 29, CdsCode = "19647826009001", Name = "Pasadena High", DistrictName = "Pasadena Unified", Address = "2925 E Sierra Madre Blvd", City = "Pasadena", GradeSpan = "9-12" },
        new() { Id = 30, CdsCode = "19647826009002", Name = "John Muir High", DistrictName = "Pasadena Unified", Address = "1905 N Lincoln Ave", City = "Pasadena", GradeSpan = "9-12" },

        // Glendale Unified Schools (1964857)
        new() { Id = 31, CdsCode = "19648576010001", Name = "Glendale High", DistrictName = "Glendale Unified", Address = "1440 E Broadway", City = "Glendale", GradeSpan = "9-12" },
        new() { Id = 32, CdsCode = "19648576010002", Name = "Hoover High", DistrictName = "Glendale Unified", Address = "651 Glenwood Rd", City = "Glendale", GradeSpan = "9-12" },

        // Santa Monica-Malibu Unified Schools (1965060)
        new() { Id = 33, CdsCode = "19650606011001", Name = "Santa Monica High", DistrictName = "Santa Monica-Malibu Unified", Address = "601 Pico Blvd", City = "Santa Monica", GradeSpan = "9-12" },
        new() { Id = 34, CdsCode = "19650606011002", Name = "Malibu High", DistrictName = "Santa Monica-Malibu Unified", Address = "30215 Morning View Dr", City = "Malibu", GradeSpan = "6-12" },

        // Compton Unified Schools (1964592)
        new() { Id = 35, CdsCode = "19645926012001", Name = "Compton High", DistrictName = "Compton Unified", Address = "601 S Acacia Ave", City = "Compton", GradeSpan = "9-12" },
        new() { Id = 36, CdsCode = "19645926012002", Name = "Dominguez High", DistrictName = "Compton Unified", Address = "15301 S San Jose Ave", City = "Compton", GradeSpan = "9-12" },

        // Inglewood Unified Schools (1964691)
        new() { Id = 37, CdsCode = "19646916013001", Name = "Inglewood High", DistrictName = "Inglewood Unified", Address = "231 S Grevillea Ave", City = "Inglewood", GradeSpan = "9-12" },
        new() { Id = 38, CdsCode = "19646916013002", Name = "Morningside High", DistrictName = "Inglewood Unified", Address = "10500 Yukon Ave", City = "Inglewood", GradeSpan = "9-12" },

        // Torrance Unified Schools (1975440)
        new() { Id = 39, CdsCode = "19754406014001", Name = "Torrance High", DistrictName = "Torrance Unified", Address = "2200 W Carson St", City = "Torrance", GradeSpan = "9-12" },
        new() { Id = 40, CdsCode = "19754406014002", Name = "South High", DistrictName = "Torrance Unified", Address = "4801 Pacific Coast Hwy", City = "Torrance", GradeSpan = "9-12" },

        // Sweetwater Union High Schools (3768171)
        new() { Id = 41, CdsCode = "37681716015001", Name = "Sweetwater High", DistrictName = "Sweetwater Union High", Address = "2900 Highland Ave", City = "National City", GradeSpan = "9-12" },
        new() { Id = 42, CdsCode = "37681716015002", Name = "Bonita Vista High", DistrictName = "Sweetwater Union High", Address = "751 Otay Lakes Rd", City = "Chula Vista", GradeSpan = "9-12" },

        // Poway Unified Schools (3768049)
        new() { Id = 43, CdsCode = "37680496016001", Name = "Poway High", DistrictName = "Poway Unified", Address = "15500 Espola Rd", City = "Poway", GradeSpan = "9-12" },
        new() { Id = 44, CdsCode = "37680496016002", Name = "Rancho Bernardo High", DistrictName = "Poway Unified", Address = "13010 Paseo Lucido", City = "San Diego", GradeSpan = "9-12" },

        // Chula Vista Elementary Schools (3768080)
        new() { Id = 45, CdsCode = "37680806017001", Name = "Chula Vista Hills Elementary", DistrictName = "Chula Vista Elementary", Address = "990 E J St", City = "Chula Vista", GradeSpan = "K-6" },
        new() { Id = 46, CdsCode = "37680806017002", Name = "Hilltop Drive Elementary", DistrictName = "Chula Vista Elementary", Address = "44 E J St", City = "Chula Vista", GradeSpan = "K-6" },

        // Anaheim Union High Schools (3066399)
        new() { Id = 47, CdsCode = "30663996018001", Name = "Anaheim High", DistrictName = "Anaheim Union High", Address = "811 W Lincoln Ave", City = "Anaheim", GradeSpan = "9-12" },
        new() { Id = 48, CdsCode = "30663996018002", Name = "Katella High", DistrictName = "Anaheim Union High", Address = "2200 E Wagner Ave", City = "Anaheim", GradeSpan = "9-12" },

        // Garden Grove Unified Schools (3066464)
        new() { Id = 49, CdsCode = "30664646019001", Name = "Garden Grove High", DistrictName = "Garden Grove Unified", Address = "11271 Stanford Ave", City = "Garden Grove", GradeSpan = "9-12" },
        new() { Id = 50, CdsCode = "30664646019002", Name = "Santiago High", DistrictName = "Garden Grove Unified", Address = "12342 Trask Ave", City = "Garden Grove", GradeSpan = "9-12" },

        // Irvine Unified Schools (3066670)
        new() { Id = 51, CdsCode = "30666706020001", Name = "Irvine High", DistrictName = "Irvine Unified", Address = "4321 Walnut Ave", City = "Irvine", GradeSpan = "9-12" },
        new() { Id = 52, CdsCode = "30666706020002", Name = "University High", DistrictName = "Irvine Unified", Address = "4771 Campus Dr", City = "Irvine", GradeSpan = "9-12" },

        // Central Unified Schools (1062000)
        new() { Id = 53, CdsCode = "10620006021001", Name = "Central High", DistrictName = "Central Unified", Address = "3535 N Cornelia Ave", City = "Fresno", GradeSpan = "9-12" },

        // Clovis Unified Schools (1062281)
        new() { Id = 54, CdsCode = "10622816022001", Name = "Clovis High", DistrictName = "Clovis Unified", Address = "1055 Fowler Ave", City = "Clovis", GradeSpan = "9-12" },
        new() { Id = 55, CdsCode = "10622816022002", Name = "Clovis West High", DistrictName = "Clovis Unified", Address = "1070 E Teague Ave", City = "Fresno", GradeSpan = "9-12" },

        // Elk Grove Unified Schools (3467439)
        new() { Id = 56, CdsCode = "34674396023001", Name = "Elk Grove High", DistrictName = "Elk Grove Unified", Address = "9800 Elk Grove Florin Rd", City = "Elk Grove", GradeSpan = "9-12" },
        new() { Id = 57, CdsCode = "34674396023002", Name = "Franklin High", DistrictName = "Elk Grove Unified", Address = "6400 Whitelock Pkwy", City = "Elk Grove", GradeSpan = "9-12" },

        // San Juan Unified Schools (3467272)
        new() { Id = 58, CdsCode = "34672726024001", Name = "Del Campo High", DistrictName = "San Juan Unified", Address = "4925 Dewey Dr", City = "Fair Oaks", GradeSpan = "9-12" },
        new() { Id = 59, CdsCode = "34672726024002", Name = "Rio Americano High", DistrictName = "San Juan Unified", Address = "4540 American River Dr", City = "Sacramento", GradeSpan = "9-12" },

        // Twin Rivers Unified Schools (3467355)
        new() { Id = 60, CdsCode = "34673556025001", Name = "Grant Union High", DistrictName = "Twin Rivers Unified", Address = "1400 Grand Ave", City = "Sacramento", GradeSpan = "9-12" },
        new() { Id = 61, CdsCode = "34673556025002", Name = "Foothill High", DistrictName = "Twin Rivers Unified", Address = "5000 McCloud Dr", City = "Sacramento", GradeSpan = "9-12" },

        // Fremont Unified Schools (0161143)
        new() { Id = 62, CdsCode = "01611436026001", Name = "Washington High", DistrictName = "Fremont Unified", Address = "38442 Fremont Blvd", City = "Fremont", GradeSpan = "9-12" },
        new() { Id = 63, CdsCode = "01611436026002", Name = "American High", DistrictName = "Fremont Unified", Address = "36300 Fremont Blvd", City = "Fremont", GradeSpan = "9-12" },

        // Hayward Unified Schools (0161176)
        new() { Id = 64, CdsCode = "01611766027001", Name = "Hayward High", DistrictName = "Hayward Unified", Address = "1633 East Ave", City = "Hayward", GradeSpan = "9-12" },
        new() { Id = 65, CdsCode = "01611766027002", Name = "Tennyson High", DistrictName = "Hayward Unified", Address = "27035 Whitman St", City = "Hayward", GradeSpan = "9-12" },

        // San Jose Unified Schools (4369682)
        new() { Id = 66, CdsCode = "43696826028001", Name = "San Jose High", DistrictName = "San Jose Unified", Address = "275 N 24th St", City = "San Jose", GradeSpan = "9-12" },
        new() { Id = 67, CdsCode = "43696826028002", Name = "Lincoln High", DistrictName = "San Jose Unified", Address = "555 Dana Ave", City = "San Jose", GradeSpan = "9-12" },

        // East Side Union High Schools (4369427)
        new() { Id = 68, CdsCode = "43694276029001", Name = "Evergreen Valley High", DistrictName = "East Side Union High", Address = "3300 Quimby Rd", City = "San Jose", GradeSpan = "9-12" },
        new() { Id = 69, CdsCode = "43694276029002", Name = "James Lick High", DistrictName = "East Side Union High", Address = "57 N White Rd", City = "San Jose", GradeSpan = "9-12" },

        // Riverside Unified Schools (3367124)
        new() { Id = 70, CdsCode = "33671246030001", Name = "Riverside Poly High", DistrictName = "Riverside Unified", Address = "5450 Victoria Ave", City = "Riverside", GradeSpan = "9-12" },
        new() { Id = 71, CdsCode = "33671246030002", Name = "North High", DistrictName = "Riverside Unified", Address = "1550 N Third St", City = "Riverside", GradeSpan = "9-12" },

        // Corona-Norco Unified Schools (3375093)
        new() { Id = 72, CdsCode = "33750936031001", Name = "Corona High", DistrictName = "Corona-Norco Unified", Address = "1150 W 10th St", City = "Corona", GradeSpan = "9-12" },
        new() { Id = 73, CdsCode = "33750936031002", Name = "Norco High", DistrictName = "Corona-Norco Unified", Address = "2065 Temescal Ave", City = "Norco", GradeSpan = "9-12" },

        // San Bernardino City Unified Schools (3667843)
        new() { Id = 74, CdsCode = "36678436032001", Name = "San Bernardino High", DistrictName = "San Bernardino City Unified", Address = "1850 N E St", City = "San Bernardino", GradeSpan = "9-12" },
        new() { Id = 75, CdsCode = "36678436032002", Name = "Pacific High", DistrictName = "San Bernardino City Unified", Address = "1020 Pacific St", City = "San Bernardino", GradeSpan = "9-12" },

        // Fontana Unified Schools (3667652)
        new() { Id = 76, CdsCode = "36676526033001", Name = "Fontana High", DistrictName = "Fontana Unified", Address = "9453 Citrus Ave", City = "Fontana", GradeSpan = "9-12" },
        new() { Id = 77, CdsCode = "36676526033002", Name = "Kaiser High", DistrictName = "Fontana Unified", Address = "11155 Almond Ave", City = "Fontana", GradeSpan = "9-12" },

        // Bakersfield City Elementary Schools (1563321)
        new() { Id = 78, CdsCode = "15633216034001", Name = "Chipman Junior High", DistrictName = "Bakersfield City Elementary", Address = "4100 Panorama Dr", City = "Bakersfield", GradeSpan = "7-8" },
        new() { Id = 79, CdsCode = "15633216034002", Name = "Sierra Middle", DistrictName = "Bakersfield City Elementary", Address = "5101 Commerce Dr", City = "Bakersfield", GradeSpan = "7-8" },

        // Kern High Schools (1563339)
        new() { Id = 80, CdsCode = "15633396035001", Name = "Bakersfield High", DistrictName = "Kern High", Address = "1241 G St", City = "Bakersfield", GradeSpan = "9-12" },
        new() { Id = 81, CdsCode = "15633396035002", Name = "Highland High", DistrictName = "Kern High", Address = "3535 E Belle Terrace", City = "Bakersfield", GradeSpan = "9-12" }
    };

    public static List<ECSContact> Contacts { get; } = new()
    {
        // LAUSD Contacts
        new() { Id = 1, DistrictCdsCode = "1964733", Name = "Maria Rodriguez", Email = "maria.rodriguez@lausd.net", Phone = "(213) 555-0101", Title = "Credential Program Coordinator", Role = "GMS_CONTACT" },
        new() { Id = 2, DistrictCdsCode = "1964733", Name = "James Wilson", Email = "james.wilson@lausd.net", Phone = "(213) 555-0102", Title = "Human Resources Manager", Role = "GMS_CONTACT" },
        new() { Id = 3, DistrictCdsCode = "1964733", Name = "Patricia Chen", Email = "patricia.chen@lausd.net", Phone = "(213) 555-0103", Title = "Fiscal Services Director", Role = "FISCAL" },

        // Long Beach Unified Contacts
        new() { Id = 4, DistrictCdsCode = "1964949", Name = "Robert Martinez", Email = "rmartinez@lbusd.k12.ca.us", Phone = "(562) 555-0201", Title = "Teacher Preparation Liaison", Role = "GMS_CONTACT" },
        new() { Id = 5, DistrictCdsCode = "1964949", Name = "Jennifer Davis", Email = "jdavis@lbusd.k12.ca.us", Phone = "(562) 555-0202", Title = "HR Specialist", Role = "GMS_CONTACT" },

        // San Diego Unified Contacts
        new() { Id = 6, DistrictCdsCode = "3768338", Name = "Michael Thompson", Email = "mthompson@sandi.net", Phone = "(619) 555-0301", Title = "Credential Services Manager", Role = "GMS_CONTACT" },
        new() { Id = 7, DistrictCdsCode = "3768338", Name = "Sarah Anderson", Email = "sanderson@sandi.net", Phone = "(619) 555-0302", Title = "Grant Programs Coordinator", Role = "GMS_CONTACT" },
        new() { Id = 8, DistrictCdsCode = "3768338", Name = "David Lee", Email = "dlee@sandi.net", Phone = "(619) 555-0303", Title = "Accounts Payable Manager", Role = "FISCAL" },

        // Fresno Unified Contacts
        new() { Id = 9, DistrictCdsCode = "1062166", Name = "Linda Garcia", Email = "linda.garcia@fresnounified.org", Phone = "(559) 555-0401", Title = "Human Capital Specialist", Role = "GMS_CONTACT" },
        new() { Id = 10, DistrictCdsCode = "1062166", Name = "William Brown", Email = "william.brown@fresnounified.org", Phone = "(559) 555-0402", Title = "Budget Analyst", Role = "FISCAL" },

        // Sacramento City Unified Contacts
        new() { Id = 11, DistrictCdsCode = "3467447", Name = "Elizabeth Taylor", Email = "etaylor@scusd.edu", Phone = "(916) 555-0501", Title = "Teacher Recruitment Coordinator", Role = "GMS_CONTACT" },
        new() { Id = 12, DistrictCdsCode = "3467447", Name = "Richard Moore", Email = "rmoore@scusd.edu", Phone = "(916) 555-0502", Title = "Fiscal Services Specialist", Role = "FISCAL" },

        // Natomas Unified Contacts
        new() { Id = 13, DistrictCdsCode = "3467314", Name = "Susan Johnson", Email = "sjohnson@natomasunified.org", Phone = "(916) 555-0601", Title = "HR Director", Role = "GMS_CONTACT" },

        // Santa Ana Unified Contacts
        new() { Id = 14, DistrictCdsCode = "3066621", Name = "Thomas White", Email = "twhite@sausd.us", Phone = "(714) 555-0701", Title = "Educator Preparation Coordinator", Role = "GMS_CONTACT" },

        // Irvine Unified Contacts
        new() { Id = 15, DistrictCdsCode = "3066670", Name = "Karen Harris", Email = "kharris@iusd.org", Phone = "(949) 555-0801", Title = "Teacher Development Manager", Role = "GMS_CONTACT" },
        new() { Id = 16, DistrictCdsCode = "3066670", Name = "Daniel Clark", Email = "dclark@iusd.org", Phone = "(949) 555-0802", Title = "Finance Manager", Role = "FISCAL" },

        // Oakland Unified Contacts
        new() { Id = 17, DistrictCdsCode = "0161259", Name = "Nancy Lewis", Email = "nancy.lewis@ousd.org", Phone = "(510) 555-0901", Title = "Talent Acquisition Manager", Role = "GMS_CONTACT" },

        // San Francisco Unified Contacts
        new() { Id = 18, DistrictCdsCode = "3868478", Name = "Christopher Walker", Email = "walkerc@sfusd.edu", Phone = "(415) 555-1001", Title = "Credentialing Specialist", Role = "GMS_CONTACT" },
        new() { Id = 19, DistrictCdsCode = "3868478", Name = "Amanda Hall", Email = "halla@sfusd.edu", Phone = "(415) 555-1002", Title = "Finance Director", Role = "FISCAL" },

        // Pasadena Unified Contacts
        new() { Id = 20, DistrictCdsCode = "1964782", Name = "Jennifer Lopez", Email = "jlopez@pusd.us", Phone = "(626) 555-1101", Title = "HR Coordinator", Role = "GMS_CONTACT" },
        new() { Id = 21, DistrictCdsCode = "1964782", Name = "Mark Stevens", Email = "mstevens@pusd.us", Phone = "(626) 555-1102", Title = "Budget Manager", Role = "FISCAL" },

        // Glendale Unified Contacts
        new() { Id = 22, DistrictCdsCode = "1964857", Name = "Anna Petrosyan", Email = "apetrosyan@gusd.net", Phone = "(818) 555-1201", Title = "Teacher Development Coordinator", Role = "GMS_CONTACT" },
        new() { Id = 23, DistrictCdsCode = "1964857", Name = "David Kim", Email = "dkim@gusd.net", Phone = "(818) 555-1202", Title = "Accounts Manager", Role = "FISCAL" },

        // Santa Monica-Malibu Unified Contacts
        new() { Id = 24, DistrictCdsCode = "1965060", Name = "Rachel Green", Email = "rgreen@smmusd.org", Phone = "(310) 555-1301", Title = "Credential Services Manager", Role = "GMS_CONTACT" },

        // Compton Unified Contacts
        new() { Id = 25, DistrictCdsCode = "1964592", Name = "Marcus Johnson", Email = "mjohnson@compton.k12.ca.us", Phone = "(310) 555-1401", Title = "HR Specialist", Role = "GMS_CONTACT" },
        new() { Id = 26, DistrictCdsCode = "1964592", Name = "Patricia Williams", Email = "pwilliams@compton.k12.ca.us", Phone = "(310) 555-1402", Title = "Finance Coordinator", Role = "FISCAL" },

        // Inglewood Unified Contacts
        new() { Id = 27, DistrictCdsCode = "1964691", Name = "Keisha Brown", Email = "kbrown@inglewood.k12.ca.us", Phone = "(310) 555-1501", Title = "Teacher Recruitment Specialist", Role = "GMS_CONTACT" },

        // Torrance Unified Contacts
        new() { Id = 28, DistrictCdsCode = "1975440", Name = "Brian Tanaka", Email = "btanaka@tusd.org", Phone = "(310) 555-1601", Title = "Credential Program Manager", Role = "GMS_CONTACT" },
        new() { Id = 29, DistrictCdsCode = "1975440", Name = "Lisa Chen", Email = "lchen@tusd.org", Phone = "(310) 555-1602", Title = "Budget Analyst", Role = "FISCAL" },

        // Sweetwater Union High Contacts
        new() { Id = 30, DistrictCdsCode = "3768171", Name = "Carlos Ramirez", Email = "cramirez@sweetwaterschools.org", Phone = "(619) 555-1701", Title = "HR Manager", Role = "GMS_CONTACT" },

        // Poway Unified Contacts
        new() { Id = 31, DistrictCdsCode = "3768049", Name = "Michelle Park", Email = "mpark@powayusd.com", Phone = "(858) 555-1801", Title = "Credential Services Director", Role = "GMS_CONTACT" },
        new() { Id = 32, DistrictCdsCode = "3768049", Name = "Robert Lee", Email = "rlee@powayusd.com", Phone = "(858) 555-1802", Title = "Finance Director", Role = "FISCAL" },

        // Chula Vista Elementary Contacts
        new() { Id = 33, DistrictCdsCode = "3768080", Name = "Maria Santos", Email = "msantos@cvesd.org", Phone = "(619) 555-1901", Title = "HR Coordinator", Role = "GMS_CONTACT" },

        // Anaheim Union High Contacts
        new() { Id = 34, DistrictCdsCode = "3066399", Name = "John Nguyen", Email = "jnguyen@auhsd.us", Phone = "(714) 555-2001", Title = "Teacher Programs Coordinator", Role = "GMS_CONTACT" },
        new() { Id = 35, DistrictCdsCode = "3066399", Name = "Sandra Martinez", Email = "smartinez@auhsd.us", Phone = "(714) 555-2002", Title = "Fiscal Manager", Role = "FISCAL" },

        // Garden Grove Unified Contacts
        new() { Id = 36, DistrictCdsCode = "3066464", Name = "Tran Pham", Email = "tpham@ggusd.us", Phone = "(714) 555-2101", Title = "Human Resources Specialist", Role = "GMS_CONTACT" },

        // Central Unified Contacts
        new() { Id = 37, DistrictCdsCode = "1062000", Name = "Angela Martinez", Email = "amartinez@centralusd.k12.ca.us", Phone = "(559) 555-2201", Title = "Credential Coordinator", Role = "GMS_CONTACT" },

        // Clovis Unified Contacts
        new() { Id = 38, DistrictCdsCode = "1062281", Name = "Steven Young", Email = "syoung@clovisusd.k12.ca.us", Phone = "(559) 555-2301", Title = "Teacher Development Manager", Role = "GMS_CONTACT" },
        new() { Id = 39, DistrictCdsCode = "1062281", Name = "Rebecca Torres", Email = "rtorres@clovisusd.k12.ca.us", Phone = "(559) 555-2302", Title = "Budget Specialist", Role = "FISCAL" },

        // Elk Grove Unified Contacts
        new() { Id = 40, DistrictCdsCode = "3467439", Name = "James Wilson", Email = "jwilson@egusd.net", Phone = "(916) 555-2401", Title = "HR Director", Role = "GMS_CONTACT" },

        // San Juan Unified Contacts
        new() { Id = 41, DistrictCdsCode = "3467272", Name = "Laura Adams", Email = "ladams@sanjuan.edu", Phone = "(916) 555-2501", Title = "Credential Services Coordinator", Role = "GMS_CONTACT" },
        new() { Id = 42, DistrictCdsCode = "3467272", Name = "Michael Roberts", Email = "mroberts@sanjuan.edu", Phone = "(916) 555-2502", Title = "Finance Manager", Role = "FISCAL" },

        // Twin Rivers Unified Contacts
        new() { Id = 43, DistrictCdsCode = "3467355", Name = "Diana Washington", Email = "dwashington@twinriversusd.org", Phone = "(916) 555-2601", Title = "HR Specialist", Role = "GMS_CONTACT" },

        // Fremont Unified Contacts
        new() { Id = 44, DistrictCdsCode = "0161143", Name = "Kevin Patel", Email = "kpatel@fremont.k12.ca.us", Phone = "(510) 555-2701", Title = "Teacher Programs Manager", Role = "GMS_CONTACT" },

        // Hayward Unified Contacts
        new() { Id = 45, DistrictCdsCode = "0161176", Name = "Gloria Hernandez", Email = "ghernandez@husd.k12.ca.us", Phone = "(510) 555-2801", Title = "HR Coordinator", Role = "GMS_CONTACT" },
        new() { Id = 46, DistrictCdsCode = "0161176", Name = "Anthony Chen", Email = "achen@husd.k12.ca.us", Phone = "(510) 555-2802", Title = "Budget Analyst", Role = "FISCAL" },

        // San Jose Unified Contacts
        new() { Id = 47, DistrictCdsCode = "4369682", Name = "Maria Gonzalez", Email = "mgonzalez@sjusd.org", Phone = "(408) 555-2901", Title = "Credential Services Director", Role = "GMS_CONTACT" },

        // East Side Union High Contacts
        new() { Id = 48, DistrictCdsCode = "4369427", Name = "Thanh Tran", Email = "ttran@esuhsd.org", Phone = "(408) 555-3001", Title = "HR Manager", Role = "GMS_CONTACT" },
        new() { Id = 49, DistrictCdsCode = "4369427", Name = "Jennifer Kim", Email = "jkim@esuhsd.org", Phone = "(408) 555-3002", Title = "Finance Coordinator", Role = "FISCAL" },

        // Riverside Unified Contacts
        new() { Id = 50, DistrictCdsCode = "3367124", Name = "Brandon Mitchell", Email = "bmitchell@rusd.k12.ca.us", Phone = "(951) 555-3101", Title = "Teacher Development Coordinator", Role = "GMS_CONTACT" },

        // Corona-Norco Unified Contacts
        new() { Id = 51, DistrictCdsCode = "3375093", Name = "Stephanie Davis", Email = "sdavis@cnusd.k12.ca.us", Phone = "(951) 555-3201", Title = "HR Specialist", Role = "GMS_CONTACT" },
        new() { Id = 52, DistrictCdsCode = "3375093", Name = "Chris Thompson", Email = "cthompson@cnusd.k12.ca.us", Phone = "(951) 555-3202", Title = "Budget Manager", Role = "FISCAL" },

        // San Bernardino City Unified Contacts
        new() { Id = 53, DistrictCdsCode = "3667843", Name = "Monica Rodriguez", Email = "mrodriguez@sbcusd.k12.ca.us", Phone = "(909) 555-3301", Title = "Credential Coordinator", Role = "GMS_CONTACT" },

        // Fontana Unified Contacts
        new() { Id = 54, DistrictCdsCode = "3667652", Name = "Derek Johnson", Email = "djohnson@fusd.net", Phone = "(909) 555-3401", Title = "HR Director", Role = "GMS_CONTACT" },
        new() { Id = 55, DistrictCdsCode = "3667652", Name = "Amy Williams", Email = "awilliams@fusd.net", Phone = "(909) 555-3402", Title = "Finance Manager", Role = "FISCAL" },

        // Bakersfield City Elementary Contacts
        new() { Id = 56, DistrictCdsCode = "1563321", Name = "Jorge Mendez", Email = "jmendez@bcsd.com", Phone = "(661) 555-3501", Title = "Teacher Programs Specialist", Role = "GMS_CONTACT" },

        // Kern High Contacts
        new() { Id = 57, DistrictCdsCode = "1563339", Name = "Sarah Peterson", Email = "speterson@kernhigh.org", Phone = "(661) 555-3601", Title = "Credential Services Manager", Role = "GMS_CONTACT" },
        new() { Id = 58, DistrictCdsCode = "1563339", Name = "William Chang", Email = "wchang@kernhigh.org", Phone = "(661) 555-3602", Title = "Budget Director", Role = "FISCAL" }
    };

    public static List<ECSSEIDRecord> SEIDRecords { get; } = new()
    {
        // Exact matches (will return single result)
        new() { SEID = "12345678", FirstName = "John", LastName = "Smith", DateOfBirth = new DateTime(1998, 3, 15), Last4SSN = "1234", HasActiveCredential = true },
        new() { SEID = "23456789", FirstName = "Maria", LastName = "Garcia", DateOfBirth = new DateTime(1997, 7, 22), Last4SSN = "2345", HasActiveCredential = true },
        new() { SEID = "34567890", FirstName = "David", LastName = "Chen", DateOfBirth = new DateTime(1999, 1, 10), Last4SSN = "3456", HasActiveCredential = false },
        new() { SEID = "45678901", FirstName = "Sarah", LastName = "Johnson", DateOfBirth = new DateTime(1998, 11, 5), Last4SSN = "4567", HasActiveCredential = true },
        new() { SEID = "56789012", FirstName = "Michael", LastName = "Williams", DateOfBirth = new DateTime(1997, 5, 30), Last4SSN = "5678", HasActiveCredential = true },
        new() { SEID = "67890123", FirstName = "Emily", LastName = "Brown", DateOfBirth = new DateTime(1999, 8, 18), Last4SSN = "6789", HasActiveCredential = false },
        new() { SEID = "78901234", FirstName = "Christopher", LastName = "Davis", DateOfBirth = new DateTime(1998, 2, 28), Last4SSN = "7890", HasActiveCredential = true },
        new() { SEID = "89012345", FirstName = "Jessica", LastName = "Miller", DateOfBirth = new DateTime(1997, 12, 12), Last4SSN = "8901", HasActiveCredential = true },
        new() { SEID = "90123456", FirstName = "Daniel", LastName = "Wilson", DateOfBirth = new DateTime(1999, 4, 25), Last4SSN = "9012", HasActiveCredential = false },
        new() { SEID = "01234567", FirstName = "Ashley", LastName = "Moore", DateOfBirth = new DateTime(1998, 6, 8), Last4SSN = "0123", HasActiveCredential = true },

        // Duplicate scenarios (same name, DOB, last4 - different SEID)
        // These simulate the ~700 duplicates mentioned in ECS meeting
        new() { SEID = "11111111", FirstName = "Robert", LastName = "Taylor", DateOfBirth = new DateTime(1998, 9, 15), Last4SSN = "1111", HasActiveCredential = true },
        new() { SEID = "11111112", FirstName = "Robert", LastName = "Taylor", DateOfBirth = new DateTime(1998, 9, 15), Last4SSN = "1111", HasActiveCredential = false },

        new() { SEID = "22222221", FirstName = "Jennifer", LastName = "Anderson", DateOfBirth = new DateTime(1997, 4, 20), Last4SSN = "2222", HasActiveCredential = true },
        new() { SEID = "22222222", FirstName = "Jennifer", LastName = "Anderson", DateOfBirth = new DateTime(1997, 4, 20), Last4SSN = "2222", HasActiveCredential = true },

        // Additional records
        new() { SEID = "33333333", FirstName = "Matthew", LastName = "Thomas", DateOfBirth = new DateTime(1999, 7, 4), Last4SSN = "3333", HasActiveCredential = true },
        new() { SEID = "44444444", FirstName = "Amanda", LastName = "Jackson", DateOfBirth = new DateTime(1998, 10, 31), Last4SSN = "4444", HasActiveCredential = false },
        new() { SEID = "55555555", FirstName = "Andrew", LastName = "White", DateOfBirth = new DateTime(1997, 1, 17), Last4SSN = "5555", HasActiveCredential = true },
        new() { SEID = "66666666", FirstName = "Stephanie", LastName = "Harris", DateOfBirth = new DateTime(1999, 3, 9), Last4SSN = "6666", HasActiveCredential = true },
        new() { SEID = "77777777", FirstName = "Joshua", LastName = "Martin", DateOfBirth = new DateTime(1998, 8, 23), Last4SSN = "7777", HasActiveCredential = false },
        new() { SEID = "88888888", FirstName = "Lauren", LastName = "Thompson", DateOfBirth = new DateTime(1997, 11, 11), Last4SSN = "8888", HasActiveCredential = true }
    };

    #endregion
}

#region ECS Data Models

public class ECSCounty
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class ECSDistrict
{
    public int Id { get; set; }
    public string CountyCode { get; set; } = string.Empty;
    public string CdsCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string CountyName { get; set; } = string.Empty;
}

public class ECSSchool
{
    public int Id { get; set; }
    public string CdsCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DistrictName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string GradeSpan { get; set; } = string.Empty;
}

public class ECSContact
{
    public int Id { get; set; }
    public string DistrictCdsCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // GMS_CONTACT, FISCAL, etc.
}

public class ECSSEIDRecord
{
    public string SEID { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Last4SSN { get; set; } = string.Empty;
    public bool HasActiveCredential { get; set; }
}

public enum SEIDLookupStatus
{
    NotFound,
    SingleMatch,
    MultipleMatches
}

public class SEIDLookupResult
{
    public SEIDLookupStatus Status { get; set; }
    public List<ECSSEIDRecord> Records { get; set; } = new();
}

#endregion
