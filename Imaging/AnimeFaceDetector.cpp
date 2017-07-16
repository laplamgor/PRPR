﻿#include "pch.h"
#include "AnimeFaceDetector.h"
#include "sstream"

using namespace concurrency;
using namespace cv;
using namespace std;
using namespace Imaging;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Imaging;
using namespace Windows::Storage::Streams;

AnimeFaceDetector::AnimeFaceDetector()
{
}

cv::CascadeClassifier AnimeFaceDetector::cascade;

IAsyncOperation<IVector<Windows::Foundation::Rect>^>^ AnimeFaceDetector::Detect(Windows::Foundation::Uri^ uri, double scaleFactor, int minNeighbors, Windows::Foundation::Size minSize)
{
	return create_async([this, uri, scaleFactor, minNeighbors, minSize]() {
		RandomAccessStreamReference^ streamRef = RandomAccessStreamReference::CreateFromUri(uri);

		return create_task(streamRef->OpenReadAsync()).then([](IRandomAccessStreamWithContentType^ fileStream)
		{
			return BitmapDecoder::CreateAsync(fileStream);
		}).then([](task<BitmapDecoder^> thisTask)
		{
			BitmapDecoder^ decoder = thisTask.get();
			return decoder->GetFrameAsync(0);
		}).then([this, scaleFactor, minNeighbors, minSize](BitmapFrame^ frame)
		{
			// Save some information as fields
			frameWidth = frame->PixelWidth;
			frameHeight = frame->PixelHeight;

			return frame->GetPixelDataAsync();
		}).then([this, scaleFactor, minNeighbors, minSize](PixelDataProvider^ pixelProvider)
		{
			Platform::Array<byte>^ srcPixels = pixelProvider->DetachPixelData();
			Mat im = cv::Mat(frameHeight, frameWidth, CV_8UC4);
			memcpy(im.data, srcPixels->Data, 4 * frameWidth*frameHeight);


			if (cascade.empty())
			{
				cascade.load("lbpcascade_animeface.xml");
			}

			std::vector<cv::Rect> faces;
			cascade.detectMultiScale(im, faces, scaleFactor, minNeighbors, CV_HAAR_SCALE_IMAGE, cv::Size(minSize.Width, minSize.Height));
			vector<cv::Rect>::const_iterator r = faces.begin();
			IVector<Windows::Foundation::Rect>^ facesOutput = ref new Platform::Collections::Vector<Windows::Foundation::Rect>();
			for (; r != faces.end(); ++r) {
				Windows::Foundation::Rect rect = Windows::Foundation::Rect(r->x, r->y, r->width, r->height);
				facesOutput->Append(rect);
			}

			return facesOutput;
		});
	});
}

void AnimeFaceDetector::LoadCascade() {
	if (cascade.empty())
	{
		//bool loaded = cascade.load("lbpcascade_animeface.xml");
		CascadeClassifier cc;
		{

			FileStorage mqwes("test", FileStorage::MEMORY);



			std::ostringstream fs;

			fs << "<?xml version=""1.0""?>";
			fs << "<opencv_storage><cascade><stageType>BOOST</stageType><featureType>LBP</featureType>";
			fs << "<height>80</height><width>12</width><stageParams><boostType>GAB</boostType><minHitRate>9.9500000476837158e-001</minHitRate><maxFalseAlarm>3.0000001192092896e-001</maxFalseAlarm><weightTrimRate>9.4999999999999996e-001";
			fs << "</weightTrimRate><maxDepth>1</maxDepth><maxWeakCount>100</maxWeakCount></stageParams><featureParams><maxCatCount>256</maxCatCount><featSize>1</featSize></featureParams><stageNum>16</stageNum><stages><_><maxWeakCount>4";
			fs << "</maxWeakCount><stageThreshold>-8.2867860794067383e-002</stageThreshold><weakClassifiers><_><internalNodes>";
			fs << "            0 -1 99 -268435521 -486543361 -258 1659633406 -134217857";
			fs << "            1702887279 -134217929 -184549377</internalNodes><leafValues>";
			fs << "            -7.5000000000000000e-001 8.6380833387374878e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 39 -540541017 -1060113913 -781245688 -477121697";
			fs << "            -1818664155 1105186857 -505961467 -152575569</internalNodes><leafValues>";
			fs << "            -7.9976779222488403e-001 7.5056612491607666e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 101 -479208497 -353380921 -855254781 -1566689761";
			fs << "            -454302869 1893310787 -271591561 -134222965</internalNodes><leafValues>";
			fs << "            -7.1062028408050537e-001 7.7380746603012085e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 41 -338958865 -925383977 -1438297681 -981777969";
			fs << "            -882901177 1913369038 -135286729 1995959223</internalNodes><leafValues>";
			fs << "-7.8616768121719360e-001 6.9309240579605103e-001</leafValues></_></weakClassifiers></_><_><maxWeakCount>5</maxWeakCount><stageThreshold>-7.7058833837509155e-001</stageThreshold><weakClassifiers><_><internalNodes>";
			fs << "            0 -1 14 -34089161 -2245 1878980471 -8687769 -134316045";
			fs << "            1744797563 -8388737 1795146607</internalNodes><leafValues>";
			fs << "            -6.1089491844177246e-001 7.3594772815704346e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 32 -707274321 1896302609 1132560802 -183140351 17019099";
			fs << "            830472347 -1993621429 1440074510</internalNodes><leafValues>";
			fs << "            -6.4869755506515503e-001 5.6941097974777222e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 4 -1055898237 -104492975 -1795141251 1464975384";
			fs << "            -1602043461 -914358144 1111543953 -2067496448</internalNodes><leafValues>";
			fs << "            -6.0432785749435425e-001 5.5685383081436157e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 96 -520160401 2063466495 -65665 -134217729 -50462805";
			fs << "            1761476478 1693969709 1910503031</internalNodes><leafValues>";
			fs << "            -5.6237226724624634e-001 6.2263637781143188e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 6 -1479564374 -954482597 16859161 -799804534 268468874";
			fs << "            713187329 1108033665 -714619755</internalNodes><leafValues>";
			fs << "-6.9048601388931274e-001 5.3264212608337402e-001</leafValues></_></weakClassifiers></_><_><maxWeakCount>5</maxWeakCount><stageThreshold>-7.1249550580978394e-001</stageThreshold><weakClassifiers><_><internalNodes>";
			fs << "            0 -1 21 -34638473 -553976197 -134217865 -159715533";
			fs << "            -142901385 -272629761 -8421377 -956303361</internalNodes><leafValues>";
			fs << "            -6.4170038700103760e-001 7.0683228969573975e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 100 -8389777 -185860353 -277 -2097152001 -161";
			fs << "            -209780865 -1 -529006609</internalNodes><leafValues>";
			fs << "            -5.5270516872406006e-001 6.9983023405075073e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 118 -545259537 -276857217 -1258291302 1652358910";
			fs << "            -134236308 1735819126 -16812809 -221249673</internalNodes><leafValues>";
			fs << "            -5.6243920326232910e-001 6.2150186300277710e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 19 -342885713 -1369882213 -2079215310 -765214587";
			fs << "            -2113207945 1074365452 1393631959 1409022707</internalNodes><leafValues>";
			fs << "            -6.8943935632705688e-001 5.3469669818878174e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 23 -506991005 1360417115 -1844809365 -821575604";
			fs << "            21178499 986120459 1347943419 -969541850</internalNodes><leafValues>";
			fs << "-6.7428857088088989e-001 5.5008578300476074e-001</leafValues></_></weakClassifiers></_><_><maxWeakCount>6</maxWeakCount><stageThreshold>-3.0183684825897217e-001</stageThreshold><weakClassifiers><_><internalNodes>";
			fs << "            0 -1 31 -144703505 -143130625 -17 -134381841 -143130625";
			fs << "            2012741567 -134218802 -134217841</internalNodes><leafValues>";
			fs << "            -5.3079712390899658e-001 7.5616836547851563e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 35 -137887809 -1924805943 1363218446 -817782134";
			fs << "            1099022547 1082327168 -1279204784 1128784467</internalNodes><leafValues>";
			fs << "            -6.4090979099273682e-001 5.3444361686706543e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 15 -786433589 -515129128 277173650 -132673121";
			fs << "            -884037451 1137229866 1938662135 -676336865</internalNodes><leafValues>";
			fs << "            -5.2920126914978027e-001 5.9623366594314575e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 92 -1897400451 -1627924747 -335548553 -1 1257762559";
			fs << "            -2113929417 -419433067 -235309193</internalNodes><leafValues>";
			fs << "            -5.5294114351272583e-001 5.8814722299575806e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 112 -187176146 1743897116 -1878957040 542033563";
			fs << "            1372582934 823282242 -158609727 -779295046</internalNodes><leafValues>";
			fs << "            -6.8665105104446411e-001 4.4378995895385742e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 9 1676637640 1887961346 16875658 1977614736 1682145753";
			fs << "            813744265 -842338550 1930548135</internalNodes><leafValues>";
			fs << "-7.5830078125000000e-001 3.9562159776687622e-001</leafValues></_></weakClassifiers></_><_><maxWeakCount>8</maxWeakCount><stageThreshold>-3.9228534698486328e-001</stageThreshold><weakClassifiers><_><internalNodes>";
			fs << "            0 -1 25 -167774345 -6689161 -2097153 -4194541 -282329093 -1";
			fs << "            -1 -352323601</internalNodes><leafValues>";
			fs << "            -4.7727271914482117e-001 7.4114018678665161e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 2 -1051598753 -1005571964 1900827102 2065404120";
			fs << "            -1207262247 -120553331 -1725955392 -494812414</internalNodes><leafValues>";
			fs << "            -5.2365595102310181e-001 5.3981113433837891e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 116 -2142770433 -1601462143 16842760 -804892128 1032369";
			fs << "            268763273 1091011104 -1142957585</internalNodes><leafValues>";
			fs << "            -4.7790464758872986e-001 5.4881525039672852e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 87 -532155537 1351188929 1073823759 -1253637875";
			fs << "            -721321497 -662691837 -955278809 1623500836</internalNodes><leafValues>";
			fs << "            -6.8072116374969482e-001 3.7135115265846252e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 113 -1996457508 -2146282492 -1728016135 -578347007";
			fs << "            -1609004859 193626505 1153570968 -1920333632</internalNodes><leafValues>";
			fs << "            -5.7289212942123413e-001 4.6210876107215881e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 56 -972008109 -691003372 -2147413749 2098355010";
			fs << "            143009971 -1744174583 -1073051430 617488921</internalNodes><leafValues>";
			fs << "            -5.9549087285995483e-001 4.8842963576316833e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 48 26 1971388449 419479901 2080931848 -1140292918";
			fs << "            -1719074813 -2130476842 -268398592</internalNodes><leafValues>";
			fs << "            -5.8355164527893066e-001 4.7890499234199524e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 57 -1052266874 167813132 -2130690045 -703061621";
			fs << "            -131874777 -662142838 -1064730555 1119947703</internalNodes><leafValues>";
			fs << "-6.9379311800003052e-001 3.9936643838882446e-001</leafValues></_></weakClassifiers></_><_><maxWeakCount>9</maxWeakCount><stageThreshold>-6.6581231355667114e-001</stageThreshold><weakClassifiers><_><internalNodes>";
			fs << "            0 -1 29 2080314175 -112910205 805323551 1024016674";
			fs << "            1073891387 -2137847805 1653140111 -7676933</internalNodes><leafValues>";
			fs << "            -5.5957448482513428e-001 5.4044550657272339e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 94 -1358956801 -100880986 -1887436809 1073741823";
			fs << "            -1896350220 -838860811 268434686 -1912602633</internalNodes><leafValues>";
			fs << "            -4.3124794960021973e-001 5.6135851144790649e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 76 -26230993 1357905647 -1358958674 -135266305 -524434";
			fs << "            -176291841 -142622837 -1005125829</internalNodes><leafValues>";
			fs << "            -4.6799373626708984e-001 5.1660954952239990e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 30 -313836176 -742240245 16818511 -1391787262";
			fs << "            1632363443 -156630911 -83631445 248984215</internalNodes><leafValues>";
			fs << "            -6.2023061513900757e-001 3.9792594313621521e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 91 -612895966 591778561 1073812490 369347088";
			fs << "            -1870223303 556335107 553910792 1907094058</internalNodes><leafValues>";
			fs << "            -6.2148678302764893e-001 4.1758581995964050e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 46 -1430257749 -672663689 -218104082 -135266322";
			fs << "            -1493174275 -873463809 -276826113 -690006715</internalNodes><leafValues>";
			fs << "            -5.1617449522018433e-001 5.2012032270431519e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 123 1088746207 1489289603 16781456 -443461355";
			fs << "            -762795606 -670564192 -1465814774 -101527550</internalNodes><leafValues>";
			fs << "            -5.0202989578247070e-001 5.0987190008163452e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 53 -1001679641 -955695103 25248080 -738078457 671123502";
			fs << "            193003713 -1836523327 -216026117</internalNodes><leafValues>";
			fs << "            -5.2692401409149170e-001 5.3243070840835571e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 89 2147417937 -1048642 -1039 -1766457361 -134236382";
			fs << "            -1922646177 -16777473 -1534591162</internalNodes><leafValues>";
			fs << "-4.6150138974189758e-001 5.6634509563446045e-001</leafValues></_></weakClassifiers></_><_><maxWeakCount>8</maxWeakCount><stageThreshold>-1.2349532842636108e+000</stageThreshold><weakClassifiers><_><internalNodes>";
			fs << "            0 -1 67 -142902409 -67142273 1878982639 -1182802113 -75841";
			fs << "            -274219146 -88604929 -31817921</internalNodes><leafValues>";
			fs << "            -4.5625588297843933e-001 5.7534247636795044e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 128 -808330661 1390004234 1107406871 -2098932967";
			fs << "            -767440829 1208655939 -1971196977 1351600587</internalNodes><leafValues>";
			fs << "            -5.7236993312835693e-001 4.1942635178565979e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 0 -805307409 -1052697 -65684 -4233 -134217745 -4194453";
			fs << "            -696778831 -708062879</internalNodes><leafValues>";
			fs << "            -4.5485407114028931e-001 5.5909335613250732e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 119 -169888509 1150652435 1074791064 541757442";
			fs << "            -645182635 989929472 1262741126 1963976639</internalNodes><leafValues>";
			fs << "            -6.4869618415832520e-001 3.9796143770217896e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 38 -912524801 811171970 33644801 -717151469 -2108956437";
			fs << "            294158344 1109713681 1900266000</internalNodes><leafValues>";
			fs << "            -5.0387507677078247e-001 5.1329559087753296e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 20 -746687625 -200802301 1073872962 285491202";
			fs << "            1208512717 -2138664446 -1837102693 1174835902</internalNodes><leafValues>";
			fs << "            -5.9465301036834717e-001 4.4057011604309082e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 16 -442903927 -988184502 -717209211 1443168395";
			fs << "            -1465793521 1252524168 1107337938 -1050414557</internalNodes><leafValues>";
			fs << "            -5.9043467044830322e-001 4.3687704205513000e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 104 -1692667790 -612286452 -1056931520 437452806";
			fs << "            -2136309078 -401536992 -1987928929 -1033981310</internalNodes><leafValues>";
			fs << "-5.0495445728302002e-001 4.9910807609558105e-001</leafValues></_></weakClassifiers></_><_><maxWeakCount>9</maxWeakCount><stageThreshold>-5.4583048820495605e-001</stageThreshold><weakClassifiers><_><internalNodes>";
			fs << "            0 -1 97 -419954689 -570949699 2147417599 -1 -872415749";
			fs << "            -301989897 -872433670 -268443689</internalNodes><leafValues>";
			fs << "            -4.0734556317329407e-001 7.1092438697814941e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 3 -1062674253 1929486475 197402 1841550219 135268235";
			fs << "            -1165491808 956369290 1258896162</internalNodes><leafValues>";
			fs << "            -5.4886269569396973e-001 4.1644170880317688e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 37 -620271105 -901300206 1359008346 -603537150";
			fs << "            1355455189 596312193 -247999129 -728767550</internalNodes><leafValues>";
			fs << "            -5.1914668083190918e-001 3.9419922232627869e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 17 -1072700149 546031429 12798103 1881656595 35238042";
			fs << "            682232321 176931799 1148695251</internalNodes><leafValues>";
			fs << "            -5.4100900888442993e-001 4.0588796138763428e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 71 -522857685 1350893957 17339597 1999601732 -779974469";
			fs << "            -359071607 1879296642 -1236927697</internalNodes><leafValues>";
			fs << "            -4.9249285459518433e-001 4.4877073168754578e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 93 2037497904 492944831 -2013291075 -754983169";
			fs << "            1837104414 -671812233 -1660989976 -973105033</internalNodes><leafValues>";
			fs << "            -4.6483671665191650e-001 4.8267844319343567e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 33 -553943182 -100663369 -1327169 -181207174 -805896236";
			fs << "            -16777225 -32770 -344459717</internalNodes><leafValues>";
			fs << "            -3.9679497480392456e-001 5.6408804655075073e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 44 -8439301 -9502850 2147412095 2134171367 1467968283";
			fs << "            -555876513 1719612907 -959121</internalNodes><leafValues>";
			fs << "            -3.7275579571723938e-001 6.2219065427780151e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 62 -2086686357 -2143072184 1073745988 -1878839231";
			fs << "            1221503177 -2113732606 1133091218 1470880455</internalNodes><leafValues>";
			fs << "-5.5160778760910034e-001 4.4197219610214233e-001</leafValues></_></weakClassifiers></_><_><maxWeakCount>8</maxWeakCount><stageThreshold>-4.9482953548431396e-001</stageThreshold><weakClassifiers><_><internalNodes>";
			fs << "            0 -1 124 803987455 -1207959557 -1073747969 -3 -1879048193";
			fs << "            -1720221705 -1073744641 -1212159499</internalNodes><leafValues>";
			fs << "            -4.2883211374282837e-001 5.8106172084808350e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 1 -1520569905 -125497088 1360134399 -49444069";
			fs << "            -1065189105 -612134877 -1497194288 -1006112575</internalNodes><leafValues>";
			fs << "            -4.8296096920967102e-001 4.3431344628334045e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 108 -67112229 -797503462 268623881 1083056391";
			fs << "            -1874187198 1879638016 -804355463 1985162053</internalNodes><leafValues>";
			fs << "            -6.1597704887390137e-001 3.4508374333381653e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 26 -686760009 1468434576 1140918535 -880733942 12599987";
			fs << "            -1304752000 -1593784081 115557220</internalNodes><leafValues>";
			fs << "            -5.7973521947860718e-001 4.0324980020523071e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 115 -753405796 4259842 -872415136 85172613 154534824";
			fs << "            8454145 -2147292968 1094185899</internalNodes><leafValues>";
			fs << "            -4.7171372175216675e-001 4.6018373966217041e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 64 -737160572 2107229470 1478238399 386729999 46739708";
			fs << "            -1717532540 134302191 1502456202</internalNodes><leafValues>";
			fs << "            -4.7625115513801575e-001 4.6307522058486938e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 63 574973114 1079378118 151608 -1089433600 683881170";
			fs << "            1234370560 25761968 1305471639</internalNodes><leafValues>";
			fs << "            -5.4804503917694092e-001 4.2817059159278870e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 126 -913048353 -1333444591 303141015 1107341569";
			fs << "            -1727960821 1644167297 -1190753878 1418524891</internalNodes><leafValues>";
			fs << "-6.3843786716461182e-001 3.2018747925758362e-001</leafValues></_></weakClassifiers></_><_><maxWeakCount>10</maxWeakCount><stageThreshold>-4.7552201151847839e-001</stageThreshold><weakClassifiers><_><internalNodes>";
			fs << "            0 -1 54 -17825929 -8718489 -34111631 -135004289 -1358954497";
			fs << "            -16814213 -151556225 -285220369</internalNodes><leafValues>";
			fs << "            -4.1965106129646301e-001 5.5681818723678589e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 88 -1856526326 -645691871 337711324 1464176998";
			fs << "            -1602581814 -1710751608 168420078 -1341468062</internalNodes><leafValues>";
			fs << "            -4.0517404675483704e-001 4.9981650710105896e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 45 -741223945 -1627185101 822169913 407916675";
			fs << "            -897447857 589300224 540099855 -1156899883</internalNodes><leafValues>";
			fs << "            -4.4794428348541260e-001 4.3524059653282166e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 66 258608606 -1120993285 -419517441 -578240642";
			fs << "            -1879056401 -1101037569 -13383 -28301584</internalNodes><leafValues>";
			fs << "            -3.9371734857559204e-001 5.2872020006179810e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 117 -350280689 -829730738 -1073461695 38377489";
			fs << "            -645158785 839057410 -1249137694 1882566387</internalNodes><leafValues>";
			fs << "            -5.7474929094314575e-001 3.8859930634498596e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 34 1536523031 -952168281 -1855975139 -854621937";
			fs << "            -939095838 -1744699368 -796270511 1582955555</internalNodes><leafValues>";
			fs << "            -5.4318642616271973e-001 4.1631007194519043e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 51 1393782562 319525363 8471383 1368384004 889651722";
			fs << "            1921550554 -1836930098 1660195204</internalNodes><leafValues>";
			fs << "            -7.2387772798538208e-001 2.8236424922943115e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 78 1675075922 637567168 -2130116204 -1890844654";
			fs << "            34255055 167907336 1091555477 -2142773065</internalNodes><leafValues>";
			fs << "            -5.3113341331481934e-001 3.7920853495597839e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 7 1164149387 1433912608 16876979 1595080980 1275865262";
			fs << "            -1446313974 1241665562 173580528</internalNodes><leafValues>";
			fs << "            -5.0643980503082275e-001 4.4159597158432007e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 129 -111949961 -783789413 268583504 -923765997";
			fs << "            -1073657336 -1340440574 -394149886 1216081042</internalNodes><leafValues>";
			fs << "-5.0880813598632813e-001 4.1170257329940796e-001</leafValues></_></weakClassifiers></_><_><maxWeakCount>11</maxWeakCount><stageThreshold>-6.9445723295211792e-001</stageThreshold><weakClassifiers><_><internalNodes>";
			fs << "            0 -1 106 -487588613 -118095873 -1 2109472735 -1258291202";
			fs << "            -101712129 -33832963 -67652237</internalNodes><leafValues>";
			fs << "            -4.0311419963836670e-001 6.2951332330703735e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 49 -268435473 -353372166 2138045906 -4121 -276824105";
			fs << "            1317007308 -41945099 -134484017</internalNodes><leafValues>";
			fs << "            -3.5493713617324829e-001 5.5815106630325317e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 5 1460877355 -15613689 558207061 -1623109371";
			fs << "            -1926723379 244908044 -113047169 1414649856</internalNodes><leafValues>";
			fs << "            -5.8201593160629272e-001 3.5618588328361511e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 103 -669296387 189940185 -1860046723 -1760460773";
			fs << "            -1740078915 -931100536 276828352 -1917868015</internalNodes><leafValues>";
			fs << "            -4.2647001147270203e-001 4.6035429835319519e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 107 -2109233498 -602287230 -1054785005 1360101827";
			fs << "            1099137177 -318504822 -1341497202 232232049</internalNodes><leafValues>";
			fs << "            -4.9850422143936157e-001 4.4256457686424255e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 40 -54286241 -1608934766 286327519 -1270398764";
			fs << "            1267376258 1636335746 542720627 1966594122</internalNodes><leafValues>";
			fs << "            -5.5573022365570068e-001 3.9825862646102905e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 18 -904213325 1133543618 67508251 -714997735 1094779186";
			fs << "            160088201 872654991 -903019733</internalNodes><leafValues>";
			fs << "            -5.2738076448440552e-001 3.8662704825401306e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 70 1275766299 1347454976 150995380 -217382907";
			fs << "            1661501627 -788494333 1259046051 -1006600122</internalNodes><leafValues>";
			fs << "            -4.6260216832160950e-001 4.6852749586105347e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 121 -367803633 420562962 36765796 -502050533 1380984391";
			fs << "            268601345 536897573 -995624251</internalNodes><leafValues>";
			fs << "            -5.2821987867355347e-001 4.4226339459419250e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 68 -470086117 1069514507 -268472471 1936420849";
			fs << "            -1904232854 1475346303 -160432647 -258802070</internalNodes><leafValues>";
			fs << "            -4.5063796639442444e-001 5.2728754281997681e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 85 -698610339 -1504477166 1267372697 822280328";
			fs << "            -909606742 -561903583 -1658732533 962675013</internalNodes><leafValues>";
			fs << "-5.5067950487136841e-001 3.9346820116043091e-001</leafValues></_></weakClassifiers></_><_><maxWeakCount>9</maxWeakCount><stageThreshold>-7.5511032342910767e-001</stageThreshold><weakClassifiers><_><internalNodes>";
			fs << "            0 -1 27 -485801045 -1031585761 285212749 -1013038975";
			fs << "            427848842 -1006632832 -1039468406 -162905189</internalNodes><leafValues>";
			fs << "            -4.8945146799087524e-001 4.7218933701515198e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 114 -962887670 1547862275 -1827077881 1140871689";
			fs << "            -536829941 -763363328 -264142181 1112595267</internalNodes><leafValues>";
			fs << "            -6.1379230022430420e-001 3.4447920322418213e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 111 -784109321 320069633 1073811463 1074292770";
			fs << "            -2138957664 -2130001880 -2147252214 315289683</internalNodes><leafValues>";
			fs << "            -5.6861025094985962e-001 3.7049382925033569e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 80 -679857295 -17928596 -328961 991442748 1064728144";
			fs << "            -357040523 -1082493190 -1368229638</internalNodes><leafValues>";
			fs << "            -3.9095887541770935e-001 6.0248941183090210e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 82 175736687 -17072405 2130705262 -218107907";
			fs << "            -1358978530 1692925804 787824558 -672137257</internalNodes><leafValues>";
			fs << "            -4.0445902943611145e-001 6.0857713222503662e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 47 -985116365 -553647839 420626839 1968635918";
			fs << "            -1576924981 -360119808 142606465 -795508656</internalNodes><leafValues>";
			fs << "            -4.8094493150711060e-001 5.1770961284637451e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 50 -1459109750 33792144 21514342 1343230978 1124110539";
			fs << "            50364672 441024643 -202393597</internalNodes><leafValues>";
			fs << "            -5.2261912822723389e-001 4.6680617332458496e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 98 -259008926 1378975745 -1476362162 1888485505";
			fs << "            1082744897 571146241 1367392642 -1073229683</internalNodes><leafValues>";
			fs << "            -6.1712646484375000e-001 3.8970091938972473e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 125 34318799 1090695442 25199491 1342177299 -2060943181";
			fs << "            143360000 -2097010032 -907873592</internalNodes><leafValues>";
			fs << "-5.3400212526321411e-001 4.4268184900283813e-001</leafValues></_></weakClassifiers></_><_><maxWeakCount>10</maxWeakCount><stageThreshold>-4.8388049006462097e-001</stageThreshold><weakClassifiers><_><internalNodes>";
			fs << "            0 -1 120 -1477443585 -1140940929 -1342185476 1308588029";
			fs << "            -1376256001 218070525 1073741181 -41951875</internalNodes><leafValues>";
			fs << "            -5.0602412223815918e-001 5.5081558227539063e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 36 -73936261 -2137816955 -1073659749 -553533419";
			fs << "            -1073706765 -30799693 -972443088 1998113303</internalNodes><leafValues>";
			fs << "            -4.8420175909996033e-001 4.5527526736259460e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 77 454566983 420696071 16777221 -2130608117 -1719576352";
			fs << "            -644874174 -2111166071 577795078</internalNodes><leafValues>";
			fs << "            -6.1467814445495605e-001 3.4610831737518311e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 60 -1592753970 -251404269 570458176 486621571";
			fs << "            -2130476982 -1207431030 25803086 -2029039551</internalNodes><leafValues>";
			fs << "            -5.2004736661911011e-001 4.5498979091644287e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 72 694105913 1907355278 -37129 821280759 931135417";
			fs << "            -923336907 1073716718 -68419540</internalNodes><leafValues>";
			fs << "            -4.1492795944213867e-001 5.7309722900390625e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 79 1393265851 -1032732526 264196 -920530793 754211";
			fs << "            169623560 1149456611 1135983235</internalNodes><leafValues>";
			fs << "            -5.1638025045394897e-001 4.7242832183837891e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 73 706130001 -1708251305 1056944760 1006373626";
			fs << "            -1303178409 -813991949 -1183128387 -604048669</internalNodes><leafValues>";
			fs << "            -4.1649991273880005e-001 5.9589266777038574e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 95 -904859491 -134017015 1090589192 -587038719";
			fs << "            -167673709 -897449815 152141841 886696449</internalNodes><leafValues>";
			fs << "            -6.4827072620391846e-001 3.5843926668167114e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 90 -717057392 690163912 822149263 65803 -1706982525";
			fs << "            -1736400884 534537 -1630082545</internalNodes><leafValues>";
			fs << "            -5.0309199094772339e-001 5.1634097099304199e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 12 -1366843350 -2126376671 1041 -566034432 142770176";
			fs << "            12583104 51712 1116198165</internalNodes><leafValues>";
			fs << "-7.9860860109329224e-001 3.1541401147842407e-001</leafValues></_></weakClassifiers></_><_><maxWeakCount>10</maxWeakCount><stageThreshold>-5.6616169214248657e-001</stageThreshold><weakClassifiers><_><internalNodes>";
			fs << "            0 -1 28 -143395977 2004844407 -32897 1840447419 -852257";
			fs << "            -4097 -272630497 -1165502065</internalNodes><leafValues>";
			fs << "            -4.4186046719551086e-001 5.1379764080047607e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 8 -519577109 -427718635 -1862262703 -65943231 9163380";
			fs << "            1112064264 553714225 1157599521</internalNodes><leafValues>";
			fs << "            -6.9529622793197632e-001 2.9373377561569214e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 109 990036221 -1392408495 85 -1455423472 537079956";
			fs << "            -1451032448 -2121658180 -1917118335</internalNodes><leafValues>";
			fs << "            -4.6548900008201599e-001 4.4904062151908875e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 83 -307263958 1726969598 602799716 -587284627";
			fs << "            -2110304757 -1500547078 1400237979 -194002951</internalNodes><leafValues>";
			fs << "            -4.4492045044898987e-001 5.2867370843887329e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 84 -696132137 331497536 -1868546039 -1859480056";
			fs << "            1753940107 -1029504896 -1341584891 937520647</internalNodes><leafValues>";
			fs << "            -4.9129620194435120e-001 4.4696673750877380e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 61 -1056718371 -912911872 67113021 1498447874 134777514";
			fs << "            -1412955989 -2138406733 1082270464</internalNodes><leafValues>";
			fs << "            -5.8106380701065063e-001 4.1291686892509460e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 43 -648808770 -703963135 -2147401712 -1858043831";
			fs << "            1073823883 1074266248 159924795 1879588907</internalNodes><leafValues>";
			fs << "            -5.2166140079498291e-001 4.6159252524375916e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 65 538123210 285607041 -2122121208 -1651965941";
			fs << "            -1047953261 1661077920 591915 1689841382</internalNodes><leafValues>";
			fs << "            -7.4180144071578979e-001 3.0022916197776794e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 55 805390529 407044123 285213203 211421255 -1702852378";
			fs << "            -1919942528 -2134294375 2066729839</internalNodes><leafValues>";
			fs << "            -4.8658525943756104e-001 5.4231238365173340e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 69 -490280822 -1274937328 268439820 1359003776";
			fs << "            -931126870 1220674050 268681287 1997226373</internalNodes><leafValues>";
			fs << "-5.6268626451492310e-001 4.5061412453651428e-001</leafValues></_></weakClassifiers></_><_><maxWeakCount>10</maxWeakCount><stageThreshold>-9.9649858474731445e-001</stageThreshold><weakClassifiers><_><internalNodes>";
			fs << "            0 -1 122 -1745100805 -1209164803 -1073770531 -436207891";
			fs << "            -1090560009 234354687 -1610664449 -1082138881</internalNodes><leafValues>";
			fs << "            -4.0143370628356934e-001 5.6573116779327393e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 11 -644493203 -1021149047 16847288 -804977263";
			fs << "            1074438223 1375879170 1099505907 -233072125</internalNodes><leafValues>";
			fs << "            -4.9022576212882996e-001 4.1356840729713440e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 110 -1092637138 -1127253650 -604013462 309325799";
			fs << "            511047567 -562074754 -700452946 -763371997</internalNodes><leafValues>";
			fs << "            -4.2038223147392273e-001 5.0647193193435669e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 24 1223739637 -1419051417 1043595135 -215335105";
			fs << "            376670206 -167870465 -4194306 -222771398</internalNodes><leafValues>";
			fs << "            -4.0432786941528320e-001 5.9335744380950928e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 75 -1761937577 -1076383745 -286361737 -9060559";
			fs << "            2013197781 2013265783 -98370 -1002109842</internalNodes><leafValues>";
			fs << "            -4.4517979025840759e-001 5.2503407001495361e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 102 1359075611 -233766656 65681 -1878048735 -1610570746";
			fs << "            1379991688 -1073689784 -221669373</internalNodes><leafValues>";
			fs << "            -4.9918147921562195e-001 4.6203434467315674e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 52 1186053495 -36241670 -268451888 519745529 175382495";
			fs << "            788381687 2147319804 1327036346</internalNodes><leafValues>";
			fs << "            -4.6265572309494019e-001 5.1841813325881958e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 59 -1040035797 1946189894 50247 -1862266624 1090519113";
			fs << "            268961800 679544907 757613389</internalNodes><leafValues>";
			fs << "            -5.5006593465805054e-001 4.4656375050544739e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 10 1610993732 -939524096 1073877397 -267910919";
			fs << "            151167146 537427968 -769096510 -181428117</internalNodes><leafValues>";
			fs << "            -5.6329357624053955e-001 4.2267900705337524e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 86 -1596021624 2047393801 -2130673584 -1856700352";
			fs << "            327207619 272728192 -2004808112 491069440</internalNodes><leafValues>";
			fs << "-6.3942277431488037e-001 3.8081073760986328e-001</leafValues></_></weakClassifiers></_><_><maxWeakCount>8</maxWeakCount><stageThreshold>-5.5261385440826416e-001</stageThreshold><weakClassifiers><_><internalNodes>";
			fs << "            0 -1 13 -648185009 -1315897313 -2139077632 1367998985";
			fs << "            1744840211 -1005502457 -935198613 -74777841</internalNodes><leafValues>";
			fs << "            -5.3191488981246948e-001 4.0654698014259338e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 105 1699432742 -1890377581 1343232064 -1039957887";
			fs << "            -2142687167 637566976 -2122282989 -460871217</internalNodes><leafValues>";
			fs << "            -5.4315727949142456e-001 3.6683899164199829e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 81 -67160267 2105388843 -1619001345 1937768302";
			fs << "            -1359003974 -1098989786 -805322771 -1874678652</internalNodes><leafValues>";
			fs << "            -3.9974156022071838e-001 5.5645257234573364e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 58 -1072656189 1095241792 16777487 -352059374 4718723";
			fs << "            1109393544 1074438486 -1848987381</internalNodes><leafValues>";
			fs << "            -5.0869542360305786e-001 4.9633875489234924e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 22 226493774 -1911816127 1091108968 26214662 26222970";
			fs << "            -1123287032 -1987040599 -882898875</internalNodes><leafValues>";
			fs << "            -6.0312920808792114e-001 3.5752627253532410e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 127 -259153461 -805273578 50364730 -1060208632";
			fs << "            -1708161014 947912705 -2147450710 80388754</internalNodes><leafValues>";
			fs << "            -6.9576680660247803e-001 3.3376914262771606e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 42 -800800303 1368954882 75795 2031108096 -2013069281";
			fs << "            212336778 538680 2064105488</internalNodes><leafValues>";
			fs << "            -5.6596046686172485e-001 4.3809539079666138e-001</leafValues></_><_><internalNodes>";
			fs << "            0 -1 74 -2108215089 1260109955 -1207926768 268812673";
			fs << "            -2146893693 167788680 55189712 -140564306</internalNodes><leafValues>";
			fs << "            -5.1393473148345947e-001 4.8148322105407715e-001</leafValues></_></weakClassifiers></_></stages><features><_><rect>";
			fs << "        0 0 1 2</rect></_><_><rect>";
			fs << "        0 0 1 3</rect></_><_><rect>";
			fs << "        0 0 2 1</rect></_><_><rect>";
			fs << "        0 0 2 11</rect></_><_><rect>";
			fs << "        0 0 3 1</rect></_><_><rect>";
			fs << "        0 0 4 4</rect></_><_><rect>";
			fs << "        0 1 1 3</rect></_><_><rect>";
			fs << "        0 1 3 5</rect></_><_><rect>";
			fs << "        0 2 1 2</rect></_><_><rect>";
			fs << "        0 2 4 17</rect></_><_><rect>";
			fs << "        0 3 1 1</rect></_><_><rect>";
			fs << "        0 4 1 1</rect></_><_><rect>";
			fs << "        0 4 1 4</rect></_><_><rect>";
			fs << "        0 4 1 18</rect></_><_><rect>";
			fs << "        0 4 2 21</rect></_><_><rect>";
			fs << "        0 5 1 1</rect></_><_><rect>";
			fs << "        0 5 1 2</rect></_><_><rect>";
			fs << "        0 5 1 4</rect></_><_><rect>";
			fs << "        0 5 2 11</rect></_><_><rect>";
			fs << "        0 6 1 2</rect></_><_><rect>";
			fs << "        0 7 1 15</rect></_><_><rect>";
			fs << "        0 7 2 18</rect></_><_><rect>";
			fs << "        0 13 3 3</rect></_><_><rect>";
			fs << "        0 13 3 19</rect></_><_><rect>";
			fs << "        0 14 2 5</rect></_><_><rect>";
			fs << "        0 14 2 14</rect></_><_><rect>";
			fs << "        0 16 3 17</rect></_><_><rect>";
			fs << "        0 17 1 6</rect></_><_><rect>";
			fs << "        0 17 2 9</rect></_><_><rect>";
			fs << "        0 18 1 6</rect></_><_><rect>";
			fs << "        0 19 2 17</rect></_><_><rect>";
			fs << "        0 21 4 13</rect></_><_><rect>";
			fs << "        0 21 4 16</rect></_><_><rect>";
			fs << "        0 22 2 8</rect></_><_><rect>";
			fs << "        0 36 1 5</rect></_><_><rect>";
			fs << "        0 40 2 12</rect></_><_><rect>";
			fs << "        0 43 1 7</rect></_><_><rect>";
			fs << "        0 46 2 10</rect></_><_><rect>";
			fs << "        0 48 1 9</rect></_><_><rect>";
			fs << "        0 48 2 4</rect></_><_><rect>";
			fs << "        0 50 1 2</rect></_><_><rect>";
			fs << "        0 56 2 3</rect></_><_><rect>";
			fs << "        0 71 1 3</rect></_><_><rect>";
			fs << "        0 74 1 2</rect></_><_><rect>";
			fs << "        0 77 1 1</rect></_><_><rect>";
			fs << "        0 77 2 1</rect></_><_><rect>";
			fs << "        1 0 1 3</rect></_><_><rect>";
			fs << "        1 0 2 1</rect></_><_><rect>";
			fs << "        1 0 3 1</rect></_><_><rect>";
			fs << "        1 2 1 1</rect></_><_><rect>";
			fs << "        1 4 1 2</rect></_><_><rect>";
			fs << "        1 4 3 23</rect></_><_><rect>";
			fs << "        1 5 2 7</rect></_><_><rect>";
			fs << "        1 9 1 1</rect></_><_><rect>";
			fs << "        1 10 2 15</rect></_><_><rect>";
			fs << "        1 12 2 7</rect></_><_><rect>";
			fs << "        1 14 2 9</rect></_><_><rect>";
			fs << "        1 25 2 18</rect></_><_><rect>";
			fs << "        1 39 2 10</rect></_><_><rect>";
			fs << "        1 71 1 3</rect></_><_><rect>";
			fs << "        2 0 1 3</rect></_><_><rect>";
			fs << "        2 0 2 1</rect></_><_><rect>";
			fs << "        2 3 1 2</rect></_><_><rect>";
			fs << "        2 4 1 5</rect></_><_><rect>";
			fs << "        2 16 3 8</rect></_><_><rect>";
			fs << "        2 18 3 14</rect></_><_><rect>";
			fs << "        2 21 2 2</rect></_><_><rect>";
			fs << "        2 22 1 4</rect></_><_><rect>";
			fs << "        2 24 1 2</rect></_><_><rect>";
			fs << "        2 64 1 5</rect></_><_><rect>";
			fs << "        3 0 2 1</rect></_><_><rect>";
			fs << "        3 1 3 25</rect></_><_><rect>";
			fs << "        3 2 3 6</rect></_><_><rect>";
			fs << "        3 3 2 11</rect></_><_><rect>";
			fs << "        3 6 1 3</rect></_><_><rect>";
			fs << "        3 17 1 11</rect></_><_><rect>";
			fs << "        3 22 3 17</rect></_><_><rect>";
			fs << "        3 23 1 4</rect></_><_><rect>";
			fs << "        3 42 1 10</rect></_><_><rect>";
			fs << "        3 52 1 6</rect></_><_><rect>";
			fs << "        3 77 1 1</rect></_><_><rect>";
			fs << "        4 0 2 2</rect></_><_><rect>";
			fs << "        4 1 1 2</rect></_><_><rect>";
			fs << "        4 2 1 1</rect></_><_><rect>";
			fs << "        5 7 2 20</rect></_><_><rect>";
			fs << "        5 12 2 19</rect></_><_><rect>";
			fs << "        5 14 1 3</rect></_><_><rect>";
			fs << "        5 19 2 15</rect></_><_><rect>";
			fs << "        6 0 1 1</rect></_><_><rect>";
			fs << "        6 0 2 1</rect></_><_><rect>";
			fs << "        6 1 2 13</rect></_><_><rect>";
			fs << "        6 5 2 5</rect></_><_><rect>";
			fs << "        6 7 2 17</rect></_><_><rect>";
			fs << "        6 10 2 7</rect></_><_><rect>";
			fs << "        6 13 2 10</rect></_><_><rect>";
			fs << "        6 14 2 13</rect></_><_><rect>";
			fs << "        6 16 2 14</rect></_><_><rect>";
			fs << "        6 19 2 7</rect></_><_><rect>";
			fs << "        6 36 1 8</rect></_><_><rect>";
			fs << "        6 39 2 7</rect></_><_><rect>";
			fs << "        6 41 2 9</rect></_><_><rect>";
			fs << "        6 44 2 2</rect></_><_><rect>";
			fs << "        6 51 2 6</rect></_><_><rect>";
			fs << "        6 77 2 1</rect></_><_><rect>";
			fs << "        7 0 1 1</rect></_><_><rect>";
			fs << "        7 9 1 2</rect></_><_><rect>";
			fs << "        7 20 1 9</rect></_><_><rect>";
			fs << "        7 23 1 4</rect></_><_><rect>";
			fs << "        7 45 1 7</rect></_><_><rect>";
			fs << "        7 77 1 1</rect></_><_><rect>";
			fs << "        8 0 1 1</rect></_><_><rect>";
			fs << "        8 47 1 11</rect></_><_><rect>";
			fs << "        8 53 1 4</rect></_><_><rect>";
			fs << "        8 77 1 1</rect></_><_><rect>";
			fs << "        9 0 1 2</rect></_><_><rect>";
			fs << "        9 0 1 15</rect></_><_><rect>";
			fs << "        9 0 1 20</rect></_><_><rect>";
			fs << "        9 2 1 3</rect></_><_><rect>";
			fs << "        9 3 1 2</rect></_><_><rect>";
			fs << "        9 6 1 3</rect></_><_><rect>";
			fs << "        9 9 1 13</rect></_><_><rect>";
			fs << "        9 13 1 2</rect></_><_><rect>";
			fs << "        9 13 1 8</rect></_><_><rect>";
			fs << "        9 19 1 16</rect></_><_><rect>";
			fs << "        9 20 1 4</rect></_><_><rect>";
			fs << "        9 25 1 4</rect></_><_><rect>";
			fs << "        9 43 1 5</rect></_><_><rect>";
			fs << "        9 48 1 4</rect></_><_><rect>";
			fs << "        9 59 1 3</rect></_><_><rect>";
			fs << "        9 61 1 5</rect></_></features></cascade></opencv_storage>";

			cv::String ss = fs.str();
			FileStorage ms(ss, FileStorage::MEMORY);
			cc.read(ms.getFirstTopLevelNode());
		}
	}
}


IAsyncOperation<IVector<Windows::Foundation::Rect>^>^ AnimeFaceDetector::DetectBitmap(BitmapFrame^ frame, double scaleFactor, int minNeighbors, Windows::Foundation::Size minSize)
{
	return create_async([this, frame, scaleFactor, minNeighbors, minSize]() {
		return create_task([this, frame]()
		{
			// Save some information as fields
			frameWidth = frame->PixelWidth;
			frameHeight = frame->PixelHeight;

			return frame->GetPixelDataAsync();
		}).then([this, scaleFactor, minNeighbors, minSize](PixelDataProvider^ pixelProvider)
		{
			Platform::Array<byte>^ srcPixels = pixelProvider->DetachPixelData();
			Mat im = cv::Mat(frameHeight, frameWidth, CV_8UC4);
			memcpy(im.data, srcPixels->Data, 4 * frameWidth*frameHeight);


			if (cascade.empty())
			{
				cascade.load("lbpcascade_animeface.xml");
			}

			std::vector<cv::Rect> faces;
			cascade.detectMultiScale(im, faces, scaleFactor, minNeighbors, CV_HAAR_SCALE_IMAGE, cv::Size(minSize.Width, minSize.Height));
			vector<cv::Rect>::const_iterator r = faces.begin();
			IVector<Windows::Foundation::Rect>^ facesOutput = ref new Platform::Collections::Vector<Windows::Foundation::Rect>();
			for (; r != faces.end(); ++r) {
				Windows::Foundation::Rect rect = Windows::Foundation::Rect(r->x, r->y, r->width, r->height);
				facesOutput->Append(rect);
			}

			return facesOutput;
		});
	});
}
