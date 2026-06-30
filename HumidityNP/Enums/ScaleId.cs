namespace HumidityNP.Enums;

/// <summary>
/// Идентификатор шкалы измерения (материал/порода).
/// Соответствует Scale ID из документации X0 Series BLE Protocol.
/// </summary>
/// <remarks>
/// Полный список материалов и пород древесины, поддерживаемых влагомерами Delmhorst X0 Series.
/// Используется в характеристиках Stored Reading и New Reading.
/// </remarks>
public enum ScaleId : ushort
{
    /// <summary>Нет материала/породы</summary>
    NoMaterial = 0,

    /// <summary>Гипсокартон</summary>
    Drywall = 1,

    /// <summary>Эталон - BDX</summary>
    ReferenceBdx = 2,

    /// <summary>Gyprock (австралийский гипсокартон)</summary>
    Gyprock = 3,

    /// <summary>Гипсобетон</summary>
    Gypcrete = 4,

    /// <summary>Штукатурка</summary>
    Plaster = 5,

    /// <summary>Бетон</summary>
    Concrete = 6,

    /// <summary>Хлопковое волокно (Lint Cotton)</summary>
    LintCotton = 7,

    /// <summary>Семена хлопка</summary>
    SeedCotton = 8,

    /// <summary>Вискозное волокно</summary>
    ViscoseRayon = 9,

    /// <summary>Шерсть</summary>
    Wool = 10,

    /// <summary>Сено</summary>
    Hay = 11,

    /// <summary>Конопля</summary>
    Hemp = 12,

    /// <summary>Хмель</summary>
    Hops = 13,

    /// <summary>Табак</summary>
    Tobacco = 14,

    /// <summary>Бразильские орехи</summary>
    BrazilNuts = 15,

    /// <summary>Кожа</summary>
    Leather = 16,

    /// <summary>Бумага</summary>
    Paper = 17,

    /// <summary>Эталон - бумага</summary>
    ReferencePaper = 18,

    /// <summary>Прессованные бумажные отходы</summary>
    BaledScrap = 19,

    /// <summary>Африканское чёрное дерево</summary>
    AfricanEbony = 20,

    /// <summary>Африканское красное дерево</summary>
    AfricanMahogany = 21,

    /// <summary>Ольха</summary>
    Alder = 22,

    /// <summary>Американский вяз</summary>
    AmericanElm = 23,

    /// <summary>Апитонг</summary>
    Apitong = 24,

    /// <summary>Осина</summary>
    Aspen = 25,

    /// <summary>Бамбук</summary>
    Bamboo = 26,

    /// <summary>Липа (Basswood)</summary>
    Basswood = 27,

    /// <summary>Берёза</summary>
    Birch = 28,

    /// <summary>Чёрная камедь</summary>
    BlackGum = 29,

    /// <summary>Чёрный орех</summary>
    BlackWalnut = 30,

    /// <summary>Бразильская вишня</summary>
    BrazilianCherry = 31,

    /// <summary>Бразильское розовое дерево</summary>
    BrazilianRosewood = 32,

    /// <summary>Бразильский орех (Ipe)</summary>
    BrazilianWalnut = 33,

    /// <summary>Бубинга</summary>
    Bubinga = 34,

    /// <summary>Цедрелла</summary>
    Cedrella = 35,

    /// <summary>Вишня</summary>
    Cherry = 36,

    /// <summary>Кокоболо</summary>
    Cocobolo = 37,

    /// <summary>Тополь (Cottonwood)</summary>
    Cottonwood = 38,

    /// <summary>Кумару</summary>
    Cumaru = 39,

    /// <summary>Кипарис</summary>
    Cypress = 40,

    /// <summary>Тёмно-красный меранти</summary>
    DarkRedMeranti = 41,

    /// <summary>Дугласова пихта</summary>
    DouglasFir = 42,

    /// <summary>Восточный красный кедр</summary>
    EasternRedCedar = 43,

    /// <summary>Энгельманова ель</summary>
    EnglemannSpruce = 44,

    /// <summary>Европейский бук</summary>
    EuropeanBeech = 45,

    /// <summary>Каркас (Hackberry)</summary>
    Hackberry = 46,

    /// <summary>Гикори</summary>
    Hickory = 47,

    /// <summary>Гондурасское красное дерево</summary>
    HondurasMahogany = 48,

    /// <summary>Ладанное дерево (Incense Cedar)</summary>
    IncenseCedar = 49,

    /// <summary>Ипе</summary>
    Ipe = 50,

    /// <summary>Жатоба</summary>
    Jatoba = 51,

    /// <summary>Керуинг</summary>
    Keruing = 52,

    /// <summary>Коа</summary>
    Koa = 53,

    /// <summary>Лиственница</summary>
    Larch = 54,

    /// <summary>Длиннолистная сосна</summary>
    LongleafPine = 55,

    /// <summary>Магнолия</summary>
    Magnolia = 56,

    /// <summary>Клён</summary>
    Maple = 57,

    /// <summary>Массарандуба</summary>
    Massaranduba = 58,

    /// <summary>Обезьяний горшок (Monkey Pot)</summary>
    MonkeyPot = 59,

    /// <summary>Мирт</summary>
    Myrtle = 60,

    /// <summary>Ориентированно-стружечная плита (OSB)</summary>
    Osb = 61,

    /// <summary>OSB - Advantech Pine</summary>
    OsbAdvantechPine = 62,

    /// <summary>OSB - Advantech Aspen</summary>
    OsbAdvantechAspen = 63,

    /// <summary>Пекан</summary>
    Pecan = 64,

    /// <summary>Филиппинское красное дерево</summary>
    PhilippineMahogany = 65,

    /// <summary>Фанера</summary>
    Plywood = 66,

    /// <summary>Жёлтая сосна (Ponderosa)</summary>
    PonderosaPine = 67,

    /// <summary>Пурпурное дерево</summary>
    Purpleheart = 68,

    /// <summary>Сосна радиата</summary>
    RadiataPine = 69,

    /// <summary>Рамин</summary>
    Ramin = 70,

    /// <summary>Красная пихта</summary>
    RedFir = 71,

    /// <summary>Красная камедь</summary>
    RedGum = 72,

    /// <summary>Красный дуб</summary>
    RedOak = 73,

    /// <summary>Секвойя (Redwood)</summary>
    Redwood = 74,

    /// <summary>Каучуковое дерево</summary>
    Rubberwood = 75,

    /// <summary>Коротколистная сосна</summary>
    ShortleafPine = 76,

    /// <summary>Ситхинская ель</summary>
    SitkaSpruce = 77,

    /// <summary>Южная жёлтая сосна</summary>
    SouthernYellowPine = 78,

    /// <summary>SPF (Spruce-Pine-Fir)</summary>
    Spf = 79,

    /// <summary>SPF - Canadian</summary>
    SpfCofi = 80,

    /// <summary>Сахарная сосна</summary>
    SugarPine = 81,

    /// <summary>Таурай</summary>
    Taurai = 82,

    /// <summary>Тик</summary>
    Teak = 83,

    /// <summary>Вирола</summary>
    Virola = 84,

    /// <summary>Западный болиголов (Canadian)</summary>
    WesternHemlockCofi = 85,

    /// <summary>Западный болиголов</summary>
    WesternHemlock = 86,

    /// <summary>Белый ясень</summary>
    WhiteAsh = 87,

    /// <summary>Белая пихта</summary>
    WhiteFir = 88,

    /// <summary>Белый дуб</summary>
    WhiteOak = 89,

    /// <summary>Белая сосна</summary>
    WhitePine = 90,

    /// <summary>Жёлтый тополь</summary>
    YellowPoplar = 91,

    /// <summary>Скандинавская сосна</summary>
    SkandFuru = 92,

    /// <summary>Скандинавская ель</summary>
    SkandGran = 93,

    /// <summary>Скандинавская берёза</summary>
    SkandBjork = 94,

    /// <summary>Альпийский ясень</summary>
    AshAlpine = 95,

    /// <summary>Горный ясень</summary>
    AshMountain = 96,

    /// <summary>Квинслендский серебристый ясень</summary>
    AshSilverQueensland = 97,

    /// <summary>Балтийский белый</summary>
    BalticWhite = 98,

    /// <summary>Белый бук</summary>
    BeechWhite = 99,

    /// <summary>Блэкбатт</summary>
    Blackbutt = 100,

    /// <summary>Чёрное дерево (Blackwood)</summary>
    Blackwood = 101,

    /// <summary>Коробка, кисточка (NSW)</summary>
    BoxBrushNsw = 102,

    /// <summary>Красный кедр</summary>
    CedarRed = 103,

    /// <summary>Белый кедр</summary>
    CedarWhite = 104,

    /// <summary>Коучвуд</summary>
    Coachwood = 105,

    /// <summary>Дугласова пихта (DIC)</summary>
    FirDouglasDic = 106,

    /// <summary>Дугласова пихта (VIC)</summary>
    FirDouglasVic = 107,

    /// <summary>Южный голубой эвкалипт</summary>
    GumBlueSouthern = 108,

    /// <summary>Манна-эвкалипт</summary>
    GumManna = 109,

    /// <summary>Лесной красный эвкалипт</summary>
    GumRedForest = 110,

    /// <summary>Речной красный эвкалипт</summary>
    GumRedRiver = 111,

    /// <summary>Розовый эвкалипт</summary>
    GumRose = 112,

    /// <summary>Блестящий эвкалипт</summary>
    GumShining = 113,

    /// <summary>Пятнистый эвкалипт</summary>
    GumSpotted = 114,

    /// <summary>Серый железнокорый эвкалипт</summary>
    IronbarkGrey = 115,

    /// <summary>Красный железнокорый эвкалипт</summary>
    IronbarkRed = 116,

    /// <summary>Джарра</summary>
    Jarrah = 117,

    /// <summary>Карри</summary>
    Karri = 118,

    /// <summary>Новозеландский каури</summary>
    KauriNewZealand = 119,

    /// <summary>Африканское красное дерево</summary>
    MahoganyAfrican = 120,

    /// <summary>Марри</summary>
    Marri = 121,

    /// <summary>Северный шёлковый дуб</summary>
    OakSilkyNorthern = 122,

    /// <summary>Белый дуб</summary>
    OakWhite = 123,

    /// <summary>Белый кипарисовая сосна</summary>
    PineCypressWhite = 124,

    /// <summary>Хуповая сосна</summary>
    PineHoop = 125,

    /// <summary>Хуоновая сосна</summary>
    PineHuon = 126,

    /// <summary>Сосна радиата (NZ)</summary>
    PineRadiataNz = 127,

    /// <summary>Сосна радиата (VIC)</summary>
    PineRadiataVic = 128,

    /// <summary>Слэшовая сосна</summary>
    PineSlash = 129,

    /// <summary>Мессмат стрингибарк</summary>
    StringybarkMessmate = 130,

    /// <summary>Талловуд</summary>
    Tallowwood = 131,

    /// <summary>Тик</summary>
    TeakDuplicate = 132,

    /// <summary>Терпентин</summary>
    Turpentine = 133,

    /// <summary>Шерстистый эвкалипт</summary>
    Woolybutt = 134,

    /// <summary>Белая берёза</summary>
    WhiteBirch = 135,

    /// <summary>Жёлтая берёза</summary>
    YellowBirch = 136,

    /// <summary>Гикори</summary>
    HickoryDuplicate = 137,

    /// <summary>Восточный белый кедр</summary>
    EWhiteCedar = 138,

    /// <summary>Западный красный кедр FC</summary>
    WRedCedarFc = 139,

    /// <summary>Чёрная вишня</summary>
    BlackCherry = 140,

    /// <summary>Красный дуб</summary>
    RedOakDuplicate = 141,

    /// <summary>Восточная белая ель</summary>
    EWhiteSpruce = 142,

    /// <summary>Чёрная ель</summary>
    BlackSpruce = 143,

    /// <summary>Красная ель</summary>
    RedSpruce = 144,

    /// <summary>Твёрдый клён</summary>
    HardMaple = 145,

    /// <summary>Мягкий клён</summary>
    SoftMaple = 146,

    /// <summary>Белый ясень</summary>
    WhiteAshDuplicate = 147,

    /// <summary>Чёрный ясень</summary>
    BlackAsh = 148,

    /// <summary>Бук</summary>
    Beech = 149,

    /// <summary>Чёрный орех</summary>
    BlackWalnutDuplicate = 150,

    /// <summary>Белый вяз</summary>
    WhiteElm = 151,

    /// <summary>Восточная белая сосна</summary>
    EWhitePine = 152,

    /// <summary>Сосна Джек</summary>
    JackPine = 153,

    /// <summary>Красная сосна</summary>
    RedPine = 154,

    /// <summary>Восточный болиголов</summary>
    EasternHemlock = 155,

    /// <summary>Западный болиголов</summary>
    WesternHemlockDuplicate = 156,

    /// <summary>Бальзамическая пихта</summary>
    BalsamFir = 157,

    /// <summary>Дугласова пихта</summary>
    DouglasFirDuplicate = 158,

    /// <summary>SPF (Canadian)</summary>
    SpfCofiDuplicate = 159,

    /// <summary>SPF (Forintek)</summary>
    SpfForintek = 160,

    /// <summary>Липа</summary>
    BasswoodDuplicate = 161,

    /// <summary>Осина</summary>
    AspenDuplicate = 162,

    /// <summary>Балау</summary>
    Balau = 163,

    /// <summary>Красный балау</summary>
    BalauRed = 164,

    /// <summary>Бинтангор</summary>
    Bintangor = 165,

    /// <summary>Битис</summary>
    Bitis = 166,

    /// <summary>Ченгал</summary>
    Chengal = 167,

    /// <summary>Дамар минак</summary>
    DamarMinyak = 168,

    /// <summary>Дуриан</summary>
    Durian = 169,

    /// <summary>Геруту</summary>
    Gerutu = 170,

    /// <summary>Кетапанг</summary>
    Ketapang = 171,

    /// <summary>Джелутонг</summary>
    Jelutong = 172,

    /// <summary>Капур</summary>
    Kapur = 173,

    /// <summary>Касай</summary>
    Kasai = 174,

    /// <summary>Кекатонг</summary>
    Kekatong = 175,

    /// <summary>Келеланг</summary>
    Keledang = 176,

    /// <summary>Кембанг</summary>
    Kembang = 177,

    /// <summary>Кемпас</summary>
    Kempas = 178,

    /// <summary>Керанджи</summary>
    Keranji = 179,

    /// <summary>Керуинг (1)</summary>
    Keruing1 = 180,

    /// <summary>Керуинг (2)</summary>
    Keruing2 = 181,

    /// <summary>Красное дерево</summary>
    Mahogany = 182,

    /// <summary>Мата улат</summary>
    MataUlat = 183,

    /// <summary>Меданг</summary>
    Medang = 184,

    /// <summary>Меллантаи</summary>
    Melantai = 185,

    /// <summary>Мелунак</summary>
    Melunak = 186,

    /// <summary>Мемписанг</summary>
    Mempisang = 187,

    /// <summary>Менгуланг</summary>
    Mengkulang = 188,

    /// <summary>Мангровый меранти</summary>
    MerantiBakau = 189,

    /// <summary>Тёмно-красный меранти (1)</summary>
    MerantiDarkRed1 = 190,

    /// <summary>Тёмно-красный меранти (2)</summary>
    MerantiDarkRed2 = 191,

    /// <summary>Тёмно-красный меранти (3)</summary>
    MerantiDarkRed3 = 192,

    /// <summary>Тёмно-красный меранти (4)</summary>
    MerantiDarkRed4 = 193,

    /// <summary>Светло-красный меранти</summary>
    MerantiLightRed = 194,

    /// <summary>Белый меранти</summary>
    MerantiWhite = 195,

    /// <summary>Жёлтый меранти</summary>
    MerantiYellow = 196,

    /// <summary>Мераван</summary>
    Merawan = 197,

    /// <summary>Мербау</summary>
    Merbau = 198,

    /// <summary>Мерпаух (1)</summary>
    Merpauh1 = 199,

    /// <summary>Мерпаух (2)</summary>
    Merpauh2 = 200,

    /// <summary>Мерсава</summary>
    Mersawa = 201,

    /// <summary>Ньято</summary>
    Nyatoh = 202,

    /// <summary>Пенарахан</summary>
    Penarahan = 203,

    /// <summary>Перупок</summary>
    Perupok = 204,

    /// <summary>Рамин</summary>
    RaminDuplicate = 205,

    /// <summary>Ренгас</summary>
    Rengas = 206,

    /// <summary>Ресак</summary>
    Resak = 207,

    /// <summary>Каучуковое дерево (Hickory)</summary>
    RubberwoodHickory = 208,

    /// <summary>Сепетир</summary>
    Sepetir = 209,

    /// <summary>Сесендок</summary>
    Sesendok = 210,

    /// <summary>Терап</summary>
    Terap = 211,

    /// <summary>Терентанг</summary>
    Terentang = 212,

    /// <summary>Туаланг</summary>
    Tualang = 213,

    /// <summary>Сахарная сосна</summary>
    PineSugar = 214,

    /// <summary>Западный красный кедр</summary>
    CedarWRed = 215,

    /// <summary>Эталон - BD2100</summary>
    ReferenceBd2100 = 216,

    /// <summary>Финики</summary>
    Dates = 217,

    /// <summary>Западный болиголов</summary>
    HemlockWestern = 218,

    /// <summary>Эталон (Reference)</summary>
    Reference = 65532,

    /// <summary>Гипсокартон (альтернативный код)</summary>
    DrywallAlt = 65533,

    /// <summary>Древесина (альтернативный код)</summary>
    WoodAlt = 65534,

    /// <summary>Неизвестный материал (используется когда код не определён)</summary>
    Unknown = 0xFFFF,
}