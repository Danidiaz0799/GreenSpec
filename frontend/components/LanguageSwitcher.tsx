'use client';

import { useLocale } from '@/lib/LocaleContext';

export default function LanguageSwitcher() {
  const { locale, setLocale } = useLocale();

  return (
    <div className="flex gap-2">
      <button
        onClick={() => setLocale('es')}
        className={`px-3 py-1 text-sm font-medium rounded-md border-2 transition-colors ${
          locale === 'es'
            ? 'bg-indigo-600 text-white border-indigo-600'
            : 'bg-white text-gray-700 border-gray-300 hover:bg-gray-50'
        }`}
      >
        🇪🇸 ES
      </button>
      <button
        onClick={() => setLocale('en')}
        className={`px-3 py-1 text-sm font-medium rounded-md border-2 transition-colors ${
          locale === 'en'
            ? 'bg-indigo-600 text-white border-indigo-600'
            : 'bg-white text-gray-700 border-gray-300 hover:bg-gray-50'
        }`}
      >
        🇺🇸 EN
      </button>
    </div>
  );
}
