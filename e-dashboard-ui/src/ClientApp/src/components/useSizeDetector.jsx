import { useLayoutEffect, useState, useRef } from 'react';

function useSizeDetector(props) {
    const {
        targetRef,
        observerOptions,
    } = props;

    const resizeHandler = useRef();

    const [size, setSize] = useState({
        width: undefined,
        height: undefined
    });

    useLayoutEffect(() => {
        resizeHandler.current = (entries) => {
            entries.forEach(entry => {
                const { width, height } = (entry && entry.contentRect) || {};

                setSize(prev => {
                    if (prev.width === width && prev.height === height) {
                        return prev;
                    }

                    return { width, height };
                })
            });
        };

        const resizeObserver = new window.ResizeObserver(resizeHandler.current);

        if (targetRef.current) {
            resizeObserver.observe(targetRef.current, observerOptions);
        }

        return () => { resizeObserver.disconnect(); };
    }, [observerOptions, targetRef]);

    return { ...size };
}

export { useSizeDetector };