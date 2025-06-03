using Microsoft.EntityFrameworkCore.Migrations;

namespace WordRace000.Data.Migrations
{
    public partial class AddWordSampleRelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Önce yeni örnek cümleleri ekleyelim
            migrationBuilder.Sql(@"
                -- Ev Eşyaları için yeni örnekler
                INSERT INTO WordSample (SampleText) VALUES
                ('I put the lamp on the table to read better.'),
                ('She bought a comfortable chair for her desk.'),
                ('The clock on the wall shows three o''clock.'),
                ('He placed the remote control next to the TV.'),
                ('The soft pillow helped me sleep better.'),
                ('I wrapped myself in a warm blanket while reading.'),
                ('The fan kept the room cool during summer.'),
                ('She arranged the bottles on the kitchen shelf.'),
                ('The mirror reflected the morning sunlight.'),
                ('I need to buy new curtains for my bedroom.'),
                ('He organized his keys in a small box.'),
                ('The phone rang while I was cooking dinner.'),
                ('She cleaned the windows on a sunny day.'),
                ('We bought a new table for the dining room.'),
                ('The chair squeaked when I sat down.');

                -- Kıyafetler için yeni örnekler
                INSERT INTO WordSample (SampleText) VALUES
                ('He wore a warm scarf on the cold winter day.'),
                ('She bought new gloves for the winter season.'),
                ('The belt matched perfectly with his shoes.'),
                ('I need new socks for my running shoes.'),
                ('She wore elegant earrings to the wedding.'),
                ('The tie complemented his suit perfectly.'),
                ('He put on his boots before going outside.'),
                ('The necklace sparkled in the evening light.'),
                ('She packed her sandals for the beach trip.'),
                ('The sweater kept me warm during winter.'),
                ('I bought a new hat for summer protection.'),
                ('She wore her favorite bracelet to the party.'),
                ('The suit was perfect for the business meeting.'),
                ('These jeans are very comfortable to wear.'),
                ('She chose a colorful scarf to match her coat.');

                -- Günlük Yaşam için yeni örnekler
                INSERT INTO WordSample (SampleText) VALUES
                ('I brush my teeth with toothpaste every morning.'),
                ('She uses her toothbrush after every meal.'),
                ('The coffee was too hot to drink immediately.'),
                ('I prefer tea with breakfast in the morning.'),
                ('He filled the water bottle before his workout.'),
                ('The alarm woke me up at seven o''clock.'),
                ('I called my friend using the phone yesterday.'),
                ('She takes the bus to work every morning.'),
                ('The car needs to be washed this weekend.'),
                ('I forgot my keys in the house this morning.'),
                ('She packed her bag for school carefully.'),
                ('The message arrived while I was working.'),
                ('Our family had dinner together last night.'),
                ('The meeting started exactly at nine.'),
                ('I took a shower after my morning walk.');
            ");

            // Şimdi kelime-cümle eşleştirmelerini yapalım
            // Meyveler (CategoryId = 1)
            migrationBuilder.Sql(@"
                -- Apple eşleştirmeleri
                INSERT INTO WordSampleWords (WordId, WordSampleId)
                SELECT w.Id, ws.Id 
                FROM Words w
                CROSS JOIN WordSample ws
                WHERE w.English = 'Apple' 
                AND ws.SampleText LIKE '%apple%';

                -- Banana eşleştirmeleri
                INSERT INTO WordSampleWords (WordId, WordSampleId)
                SELECT w.Id, ws.Id 
                FROM Words w
                CROSS JOIN WordSample ws
                WHERE w.English = 'Banana' 
                AND ws.SampleText LIKE '%banana%';

                -- Orange eşleştirmeleri
                INSERT INTO WordSampleWords (WordId, WordSampleId)
                SELECT w.Id, ws.Id 
                FROM Words w
                CROSS JOIN WordSample ws
                WHERE w.English = 'Orange' 
                AND ws.SampleText LIKE '%orange%';
            ");

            // Hayvanlar (CategoryId = 3)
            migrationBuilder.Sql(@"
                -- Cat eşleştirmeleri
                INSERT INTO WordSampleWords (WordId, WordSampleId)
                SELECT w.Id, ws.Id 
                FROM Words w
                CROSS JOIN WordSample ws
                WHERE w.English = 'Cat' 
                AND ws.SampleText LIKE '%cat%';

                -- Dog eşleştirmeleri
                INSERT INTO WordSampleWords (WordId, WordSampleId)
                SELECT w.Id, ws.Id 
                FROM Words w
                CROSS JOIN WordSample ws
                WHERE w.English = 'Dog' 
                AND ws.SampleText LIKE '%dog%';

                -- Lion eşleştirmeleri
                INSERT INTO WordSampleWords (WordId, WordSampleId)
                SELECT w.Id, ws.Id 
                FROM Words w
                CROSS JOIN WordSample ws
                WHERE w.English = 'Lion' 
                AND ws.SampleText LIKE '%lion%';
            ");

            // Ev Eşyaları (CategoryId = 5)
            migrationBuilder.Sql(@"
                -- Table eşleştirmeleri
                INSERT INTO WordSampleWords (WordId, WordSampleId)
                SELECT w.Id, ws.Id 
                FROM Words w
                CROSS JOIN WordSample ws
                WHERE w.English = 'Table' 
                AND ws.SampleText LIKE '%table%';

                -- Chair eşleştirmeleri
                INSERT INTO WordSampleWords (WordId, WordSampleId)
                SELECT w.Id, ws.Id 
                FROM Words w
                CROSS JOIN WordSample ws
                WHERE w.English = 'Chair' 
                AND ws.SampleText LIKE '%chair%';

                -- Phone eşleştirmeleri
                INSERT INTO WordSampleWords (WordId, WordSampleId)
                SELECT w.Id, ws.Id 
                FROM Words w
                CROSS JOIN WordSample ws
                WHERE w.English = 'Phone' 
                AND ws.SampleText LIKE '%phone%';
            ");

            // Sebzeler (CategoryId = 2)
            migrationBuilder.Sql(@"
                -- Carrot/Carrots eşleştirmeleri
                INSERT INTO WordSampleWords (WordId, WordSampleId)
                SELECT w.Id, ws.Id 
                FROM Words w
                CROSS JOIN WordSample ws
                WHERE w.English = 'Carrot' 
                AND (ws.SampleText LIKE '%carrot%' OR ws.SampleText LIKE '%carrots%');

                -- Tomato/Tomatoes eşleştirmeleri
                INSERT INTO WordSampleWords (WordId, WordSampleId)
                SELECT w.Id, ws.Id 
                FROM Words w
                CROSS JOIN WordSample ws
                WHERE w.English = 'Tomato' 
                AND (ws.SampleText LIKE '%tomato%' OR ws.SampleText LIKE '%tomatoes%');

                -- Cucumber/Cucumbers eşleştirmeleri
                INSERT INTO WordSampleWords (WordId, WordSampleId)
                SELECT w.Id, ws.Id 
                FROM Words w
                CROSS JOIN WordSample ws
                WHERE w.English = 'Cucumber' 
                AND (ws.SampleText LIKE '%cucumber%' OR ws.SampleText LIKE '%cucumbers%');

                -- Onion/Onions eşleştirmeleri
                INSERT INTO WordSampleWords (WordId, WordSampleId)
                SELECT w.Id, ws.Id 
                FROM Words w
                CROSS JOIN WordSample ws
                WHERE w.English = 'Onion' 
                AND (ws.SampleText LIKE '%onion%' OR ws.SampleText LIKE '%onions%');
            ");

            // Kıyafetler (CategoryId = 10)
            migrationBuilder.Sql(@"
                -- Dress eşleştirmeleri
                INSERT INTO WordSampleWords (WordId, WordSampleId)
                SELECT w.Id, ws.Id 
                FROM Words w
                CROSS JOIN WordSample ws
                WHERE w.English = 'Dress' 
                AND ws.SampleText LIKE '%dress%';

                -- Jacket eşleştirmeleri
                INSERT INTO WordSampleWords (WordId, WordSampleId)
                SELECT w.Id, ws.Id 
                FROM Words w
                CROSS JOIN WordSample ws
                WHERE w.English = 'Jacket' 
                AND ws.SampleText LIKE '%jacket%';

                -- Scarf eşleştirmeleri
                INSERT INTO WordSampleWords (WordId, WordSampleId)
                SELECT w.Id, ws.Id 
                FROM Words w
                CROSS JOIN WordSample ws
                WHERE w.English = 'Scarf' 
                AND ws.SampleText LIKE '%scarf%';

                -- Shoes eşleştirmeleri
                INSERT INTO WordSampleWords (WordId, WordSampleId)
                SELECT w.Id, ws.Id 
                FROM Words w
                CROSS JOIN WordSample ws
                WHERE w.English = 'Shoes' 
                AND ws.SampleText LIKE '%shoes%';
            ");

            // Fiiller (CategoryId = 6)
            migrationBuilder.Sql(@"
                -- Eat/Ate/Eating eşleştirmeleri
                INSERT INTO WordSampleWords (WordId, WordSampleId)
                SELECT w.Id, ws.Id 
                FROM Words w
                CROSS JOIN WordSample ws
                WHERE w.English = 'Eat' 
                AND (ws.SampleText LIKE '%eat%' OR ws.SampleText LIKE '%ate%' OR ws.SampleText LIKE '%eating%');

                -- Read/Reading eşleştirmeleri
                INSERT INTO WordSampleWords (WordId, WordSampleId)
                SELECT w.Id, ws.Id 
                FROM Words w
                CROSS JOIN WordSample ws
                WHERE w.English = 'Read' 
                AND (ws.SampleText LIKE '%read%' OR ws.SampleText LIKE '%reading%');

                -- Buy/Bought eşleştirmeleri
                INSERT INTO WordSampleWords (WordId, WordSampleId)
                SELECT w.Id, ws.Id 
                FROM Words w
                CROSS JOIN WordSample ws
                WHERE w.English = 'Buy' 
                AND (ws.SampleText LIKE '%buy%' OR ws.SampleText LIKE '%bought%');

                -- Wear/Wore eşleştirmeleri
                INSERT INTO WordSampleWords (WordId, WordSampleId)
                SELECT w.Id, ws.Id 
                FROM Words w
                CROSS JOIN WordSample ws
                WHERE w.English = 'Wear' 
                AND (ws.SampleText LIKE '%wear%' OR ws.SampleText LIKE '%wore%');
            ");

            // Felsefe (CategoryId = 7)
            migrationBuilder.Sql(@"
                -- Philosophy eşleştirmeleri
                INSERT INTO WordSampleWords (WordId, WordSampleId)
                SELECT w.Id, ws.Id 
                FROM Words w
                CROSS JOIN WordSample ws
                WHERE w.English = 'Philosophy' 
                AND ws.SampleText LIKE '%philosophy%';

                -- Ethics/Moral eşleştirmeleri
                INSERT INTO WordSampleWords (WordId, WordSampleId)
                SELECT w.Id, ws.Id 
                FROM Words w
                CROSS JOIN WordSample ws
                WHERE w.English IN ('Ethics', 'Morality') 
                AND (ws.SampleText LIKE '%ethics%' OR ws.SampleText LIKE '%moral%');
            ");

            // Bilim (CategoryId = 8)
            migrationBuilder.Sql(@"
                -- Science/Scientist eşleştirmeleri
                INSERT INTO WordSampleWords (WordId, WordSampleId)
                SELECT w.Id, ws.Id 
                FROM Words w
                CROSS JOIN WordSample ws
                WHERE w.English = 'Science' 
                AND (ws.SampleText LIKE '%science%' OR ws.SampleText LIKE '%scientist%');

                -- Theory eşleştirmeleri
                INSERT INTO WordSampleWords (WordId, WordSampleId)
                SELECT w.Id, ws.Id 
                FROM Words w
                CROSS JOIN WordSample ws
                WHERE w.English = 'Theory' 
                AND ws.SampleText LIKE '%theory%';

                -- Experiment/Research eşleştirmeleri
                INSERT INTO WordSampleWords (WordId, WordSampleId)
                SELECT w.Id, ws.Id 
                FROM Words w
                CROSS JOIN WordSample ws
                WHERE w.English IN ('Experiment', 'Research') 
                AND (ws.SampleText LIKE '%experiment%' OR ws.SampleText LIKE '%research%');
            ");

            // Günlük Yaşam (CategoryId = 9)
            migrationBuilder.Sql(@"
                -- Breakfast eşleştirmeleri
                INSERT INTO WordSampleWords (WordId, WordSampleId)
                SELECT w.Id, ws.Id 
                FROM Words w
                CROSS JOIN WordSample ws
                WHERE w.English = 'Breakfast' 
                AND ws.SampleText LIKE '%breakfast%';

                -- Work eşleştirmeleri
                INSERT INTO WordSampleWords (WordId, WordSampleId)
                SELECT w.Id, ws.Id 
                FROM Words w
                CROSS JOIN WordSample ws
                WHERE w.English = 'Work' 
                AND ws.SampleText LIKE '%work%';

                -- Shopping/Market eşleştirmeleri
                INSERT INTO WordSampleWords (WordId, WordSampleId)
                SELECT w.Id, ws.Id 
                FROM Words w
                CROSS JOIN WordSample ws
                WHERE w.English IN ('Shopping', 'Market') 
                AND (ws.SampleText LIKE '%shopping%' OR ws.SampleText LIKE '%market%');
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Önce eklediğimiz ilişkileri temizleyelim
            migrationBuilder.Sql("DELETE FROM WordSampleWords");

            // Sonra eklediğimiz yeni cümleleri silelim
            migrationBuilder.Sql(@"
                DELETE FROM WordSample 
                WHERE SampleText IN (
                    'I put the lamp on the table to read better.',
                    'She bought a comfortable chair for her desk.',
                    -- Diğer yeni eklenen cümleler...
                    'I took a shower after my morning walk.'
                );
            ");
        }
    }
} 