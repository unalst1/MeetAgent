import { useState, useRef } from 'react';

function App() {
    const [isRecording, setIsRecording] = useState(false);
    const [transcript, setTranscript] = useState("");
    const [loading, setLoading] = useState(false);

    // Ses tanıma motorunu hafızada tutmak için useRef kullanıyoruz
    const recognitionRef = useRef<any>(null);

    const toggleRecording = () => {
        if (!isRecording) {
            const SpeechRecognition = (window as any).SpeechRecognition || (window as any).webkitSpeechRecognition;

            if (!SpeechRecognition) {
                alert("Tarayıcınız bu özelliği desteklemiyor. Lütfen Chrome kullanın.");
                return;
            }

            // 1. ÖNCE NESNEYİ OLUŞTURUYORUZ
            const recognition = new SpeechRecognition();
            recognition.continuous = true;
            recognition.interimResults = true;
            recognition.lang = 'tr-TR';

            // 2. SONRA GÖREVLERİNİ (EVENTLERİ) TANIYORUZ
            recognition.onerror = (event: any) => {
                console.error("Mikrofon Hatası:", event.error);
                // alert("Ses tanıma hatası: " + event.error); // Çok sık çıkarsa bunu kapatabilirsin
            };

            recognition.onresult = (event: any) => {
                let currentTranscript = "";
                for (let i = 0; i < event.results.length; i++) {
                    currentTranscript += event.results[i][0].transcript;
                }
                setTranscript(currentTranscript);
            };

            // 3. EN SON BAŞLATIYORUZ
            recognition.start();
            recognitionRef.current = recognition;
            setIsRecording(true);
        } else {
            if (recognitionRef.current) {
                recognitionRef.current.stop();
            }
            setIsRecording(false);
        }
    };

    // Backend'e (Gemini'ye) veriyi gönderen yeni fonksiyon
    const sendToGemini = async () => {
        if (!transcript) return;

        setLoading(true);
        try {
            // NOT: Port numaran Swagger'da neyse (örn: 7154) onu kontrol et
            const response = await fetch("https://localhost:7267/api/meeting/extract-tasks", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                // Backend direkt [FromBody] string beklediği için metni JSON string olarak paketliyoruz
                body: JSON.stringify({ transcript: transcript })
            });

            if (response.ok) {
                const result = await response.json();
                console.log("Gemini Sonucu:", result);
                alert("Görevler başarıyla çıkarıldı! Console ekranından detaylara bakabilirsin.");
                // İstersen gelen sonucu transcript alanına da yazdırabilirsin:
                // setTranscript("ÇIKARILAN GÖREVLER:\n" + JSON.stringify(result, null, 2));
            } else {
                const errorData = await response.text();
                alert("Hata: " + errorData);
            }
        } catch (error) {
            console.error("Bağlantı hatası:", error);
            alert("Backend'e bağlanılamadı. API'nin çalıştığından emin ol.");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="w-80 p-4 bg-slate-50 min-h-[450px] flex flex-col font-sans">
            {/* Üst Bilgi (Header) */}
            <div className="flex items-center justify-between mb-4 pb-3 border-b border-slate-200">
                <h1 className="text-lg font-extrabold text-indigo-600 flex items-center gap-2">
                    <span className="text-2xl">🎙️</span> MeetAgent
                </h1>
                <span className="text-xs bg-indigo-100 text-indigo-800 px-2 py-1 rounded-full font-semibold">
                    v1.0
                </span>
            </div>

            {/* Canlı Metin Alanı */}
            <div className="flex-1 bg-white border border-slate-200 rounded-xl p-3 mb-4 shadow-sm overflow-y-auto flex flex-col h-64">
                {transcript ? (
                    <p className="text-sm text-slate-700 leading-relaxed whitespace-pre-wrap">{transcript}</p>
                ) : (
                    <div className="m-auto text-center flex flex-col items-center opacity-50">
                        <span className="text-4xl mb-2">💬</span>
                        <p className="text-sm text-slate-500 italic">
                            {isRecording ? "Sizi dinliyorum..." : "Toplantı sesi bekleniyor..."}
                        </p>
                    </div>
                )}
            </div>

            {/* Kontrol Butonları */}
            <div className="flex flex-col gap-3 mt-auto">
                <button
                    onClick={toggleRecording}
                    className={`w-full py-3 px-4 rounded-xl font-bold text-white transition-all shadow-md flex items-center justify-center gap-2 ${isRecording
                        ? 'bg-rose-500 hover:bg-rose-600 shadow-rose-200 animate-pulse'
                        : 'bg-indigo-600 hover:bg-indigo-700 shadow-indigo-200'
                        }`}
                >
                    {isRecording ? '🛑 Kaydı Durdur' : '▶️ Toplantıyı Dinle'}
                </button>

                <button
                    onClick={sendToGemini}
                    disabled={!transcript || loading || isRecording}
                    className={`w-full py-3 px-4 rounded-xl font-bold flex items-center justify-center gap-2 transition-all ${transcript && !loading && !isRecording
                        ? 'text-slate-700 bg-slate-200 hover:bg-slate-300 shadow-sm'
                        : 'text-slate-400 bg-slate-100 cursor-not-allowed'
                        }`}
                >
                    {loading ? '⏳ İşleniyor...' : '🚀 Özeti Çıkar & Trello\'ya At'}
                </button>
            </div>
        </div>
    );
}

export default App;