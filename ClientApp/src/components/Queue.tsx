import React, { useState, useEffect } from 'react';
import RingLoader from "react-spinners/RingLoader";
import Axios from 'axios';
import swal from 'sweetalert';

interface QueuedVideo {
    channelId: string;
    channelTitle: string;
    description: string;
    etag: string;
    highThumbnail: string;
    language: string
    publishedAt: Date;
    videoId: string;
    videoTitle: string;
    commentCount?: number;
    dislikeCount?: number;
    favoriteCount?: number;
    likeCount?: number;
    viewCount?: number;
}
export function Queue() {
    const [loading, setLoading] = useState<boolean>(false);
    const [results, setResults] = useState<QueuedVideo[]>();
    const getData = () => {
        try {
            setLoading(true);
            Axios.get('https://localhost:5001/api/queue/')
                .then((res) => {
                    setResults(res.data);
                });
        } catch (error) {
            swal({ text: 'Some error happened', timer: 5000 });
        } finally {
            setLoading(false);
        }
    };
    useEffect(() => {
        getData();
    }, []);
    const startQueue = () => {
        Axios.post('https://localhost:5001/api/queue/')
            .then((res) => {
                getData();
            });
    }
    const videos = results?.map(r =>
        <div key={r.videoId}>
            <h3>{r.videoTitle}</h3>
            <img src={r.highThumbnail} />
            <h4>{r.videoId}</h4>
            <h5>Comments: {r.commentCount}</h5>
            <h5>Dislikes: {r.dislikeCount}</h5>
            <h5>FavoriteCount: {r.favoriteCount}</h5>
            <h5>Dislikes: {r.dislikeCount}</h5>
            <h5>LikeCount: {r.likeCount}</h5>
            <h5>ViewCount: {r.viewCount}</h5>
            <br />
        </div>

    );
    return <>
        <span>Count: {videos?.length}</span>
        <button onClick={startQueue}>Start</button>
        <RingLoader
            size={150}
            color={"#123abc"}
            loading={loading}
        />
        {videos}
    </>
}