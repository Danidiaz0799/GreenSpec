import { useLocale } from './LocaleContext';
import es from '@/messages/es.json';
import en from '@/messages/en.json';

const messages = { es, en };

type Messages = typeof es;

export function useTranslations(namespace?: keyof Messages) {
  const { locale } = useLocale();
  
  return (key: string) => {
    const keys = namespace ? `${namespace}.${key}`.split('.') : key.split('.');
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    let value: any = messages[locale];
    
    for (const k of keys) {
      value = value?.[k];
    }
    
    return value || key;
  };
}
