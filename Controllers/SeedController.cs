using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WordRace000.Data;
using WordRace000.Models;

namespace WordRace000.Controllers
{
    [Authorize] // Sadece giriş yapmış kullanıcılar erişebilir
    public class SeedController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SeedController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Seed/AddSampleSentences
        public async Task<IActionResult> AddSampleSentences()
        {
            try
            {
                // Yeni örnek cümleleri ekle
                var newSentences = new[]
                {
                    // Ev Eşyaları için yeni örnekler
                    "I put the lamp on the table to read better.",
                    "She bought a comfortable chair for her desk.",
                    "The clock on the wall shows three o'clock.",
                    "He placed the remote control next to the TV.",
                    "The soft pillow helped me sleep better.",
                    "I wrapped myself in a warm blanket while reading.",
                    "The fan kept the room cool during summer.",
                    "She arranged the bottles on the kitchen shelf.",
                    "The mirror reflected the morning sunlight.",
                    "I need to buy new curtains for my bedroom.",
                    "He organized his keys in a small box.",
                    "The phone rang while I was cooking dinner.",
                    "She cleaned the windows on a sunny day.",
                    "We bought a new table for the dining room.",
                    "The chair squeaked when I sat down.",

                    // Kıyafetler için yeni örnekler
                    "He wore a warm scarf on the cold winter day.",
                    "She bought new gloves for the winter season.",
                    "The belt matched perfectly with his shoes.",
                    "I need new socks for my running shoes.",
                    "She wore elegant earrings to the wedding.",
                    "The tie complemented his suit perfectly.",
                    "He put on his boots before going outside.",
                    "The necklace sparkled in the evening light.",
                    "She packed her sandals for the beach trip.",
                    "The sweater kept me warm during winter.",
                    "I bought a new hat for summer protection.",
                    "She wore her favorite bracelet to the party.",
                    "The suit was perfect for the business meeting.",
                    "These jeans are very comfortable to wear.",
                    "She chose a colorful scarf to match her coat.",

                    // Günlük Yaşam için yeni örnekler
                    "I brush my teeth with toothpaste every morning.",
                    "She uses her toothbrush after every meal.",
                    "The coffee was too hot to drink immediately.",
                    "I prefer tea with breakfast in the morning.",
                    "He filled the water bottle before his workout.",
                    "The alarm woke me up at seven o'clock.",
                    "I called my friend using the phone yesterday.",
                    "She takes the bus to work every morning.",
                    "The car needs to be washed this weekend.",
                    "I forgot my keys in the house this morning.",
                    "She packed her bag for school carefully.",
                    "The message arrived while I was working.",
                    "Our family had dinner together last night.",
                    "The meeting started exactly at nine.",
                    "I took a shower after my morning walk."
                };

                foreach (var sentence in newSentences)
                {
                    // Cümle zaten var mı kontrol et
                    if (!await _context.WordSample.AnyAsync(ws => ws.SampleText == sentence))
                    {
                        _context.WordSample.Add(new WordSample { SampleText = sentence });
                    }
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Yeni örnek cümleler başarıyla eklendi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata oluştu: {ex.Message}";
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: Seed/CreateWordSampleRelations
        public async Task<IActionResult> CreateWordSampleRelations()
        {
            try
            {
                // Tüm kelimeleri ve cümleleri al
                var words = await _context.Words.ToListAsync();
                var samples = await _context.WordSample.ToListAsync();

                foreach (var word in words)
                {
                    // Kelimeyi küçük harfe çevir
                    var wordLower = word.English.ToLower();
                    
                    // Kelimenin çoğul formunu oluştur
                    var pluralForm = wordLower;
                    if (wordLower.EndsWith("y"))
                        pluralForm = wordLower.Substring(0, wordLower.Length - 1) + "ies";
                    else if (!wordLower.EndsWith("s"))
                        pluralForm = wordLower + "s";

                    // Fiil formları için özel kontrol
                    var verbForms = new List<string> { wordLower };
                    if (word.CategoryId == 6) // Fiiller kategorisi
                    {
                        // Geçmiş zaman formu
                        if (wordLower == "eat") verbForms.Add("ate");
                        else if (wordLower == "buy") verbForms.Add("bought");
                        else if (wordLower == "read") verbForms.Add("read");
                        else if (wordLower == "wear") verbForms.Add("wore");
                        else if (wordLower == "write") verbForms.Add("wrote");
                        else if (wordLower == "speak") verbForms.Add("spoke");
                        
                        // -ing formu
                        if (wordLower.EndsWith("e"))
                            verbForms.Add(wordLower.Substring(0, wordLower.Length - 1) + "ing");
                        else
                            verbForms.Add(wordLower + "ing");
                    }

                    // Her cümleyi kontrol et
                    foreach (var sample in samples)
                    {
                        var sampleLower = sample.SampleText.ToLower();
                        bool matches = false;

                        // Tekil form kontrolü
                        if (sampleLower.Contains(wordLower))
                            matches = true;
                        // Çoğul form kontrolü
                        else if (sampleLower.Contains(pluralForm))
                            matches = true;
                        // Fiil formları kontrolü
                        else if (verbForms.Any(form => sampleLower.Contains(form)))
                            matches = true;

                        if (matches)
                        {
                            // İlişki zaten var mı kontrol et
                            var relationExists = await _context.WordSampleWords
                                .AnyAsync(wsw => wsw.WordId == word.Id && wsw.WordSampleId == sample.Id);

                            if (!relationExists)
                            {
                                _context.WordSampleWords.Add(new WordSampleWord
                                {
                                    WordId = word.Id,
                                    WordSampleId = sample.Id
                                });
                            }
                        }
                    }
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Kelime-cümle ilişkileri başarıyla oluşturuldu.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata oluştu: {ex.Message}";
            }

            return RedirectToAction("Index", "Home");
        }
    }
} 